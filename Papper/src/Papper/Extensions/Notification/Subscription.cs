using Papper;
using Papper.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Papper.Extensions.Notification
{
    /// <summary>
    /// This representates a subscription to a plc value change detection.
    /// </summary>
    public sealed class Subscription : IDisposable, IAsyncDisposable
    {
        private readonly TaskCompletionSource<object?> _watchingTcs = new();
        private readonly PlcDataMapper _mapper;
        private readonly LruCache _lruCache = new();
        private readonly Dictionary<string, PlcWatchReference> _variables = new();
        private readonly SemaphoreSlim _semaphore;
        private readonly ChangeDetectionStrategy _changeDetectionStrategy;
        private readonly int _defaultInterval;
        private readonly AsyncAutoResetEvent<IEnumerable<DataPack>>? _changeEvent;
        private readonly AsyncAutoResetEvent<bool> _modifiedEvent = new();
        private bool _disposed;

        private CancellationTokenSource? _cts;
        private List<Execution>? _executions;
        private bool _modified = true;
        private DateTime _lastRun = DateTime.MinValue;


        /// <summary>
        /// Provides read access to the watch task.
        /// </summary>
        public Task Watching => _watchingTcs.Task;

        /// <summary>
        /// Unique Id of the subscription
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Detection interval (pause between detections)
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Returns the number of subscribed
        /// </summary>
        public int Count => _variables.Count;

        /// <summary>
        /// Returns true if the subscription has active variables.
        /// </summary>
        public bool HasVariables => _variables.Any();


        /// <summary>
        /// Create an instance of a subscription to detect plc data changes
        /// </summary>
        /// <param name="mapper">The reference to the plcDatamapper.</param>
        /// <param name="vars">The variables we should watch.</param>
        /// <param name="defaultInterval">setup the default interval, if none was given by the <see cref="PlcWatchReference"/></param>
        public Subscription(PlcDataMapper mapper, ChangeDetectionStrategy changeDetectionStrategy = ChangeDetectionStrategy.Polling, IEnumerable<PlcWatchReference>? vars = null, int defaultInterval = 1000)
        {
            _mapper = mapper ?? ExceptionThrowHelper.ThrowArgumentNullException<PlcDataMapper>(nameof(mapper));
            _changeDetectionStrategy = changeDetectionStrategy;
            _defaultInterval = defaultInterval;
            _semaphore = new SemaphoreSlim(1);
            UpdateWatchCycle(vars);

            if (changeDetectionStrategy == ChangeDetectionStrategy.Event)
            {
                _changeEvent = new AsyncAutoResetEvent<IEnumerable<DataPack>>();
            }
            if (vars != null)
            {
                // Add Items synchrone
                if(TryBuildAddableVariableList(vars, out var variables, true))
                {
                    AddValidatedVariables(variables!);
                }
            }
        }

        /// <summary>
        /// Disposing all allocations and unregister from mapper
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            using (new SemaphoreGuard(_semaphore))
            {
                _watchingTcs.TrySetResult(null);
                _mapper.RemoveSubscription(this);
                _variables.Clear();
                _lruCache.Dispose();
            }
            _semaphore.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Disposing all allocations and unregister from mapper
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            if (_semaphore != null)
            {
                using (await SemaphoreGuard.Async(_semaphore).ConfigureAwait(false))
                {
                    await InternalStopSubscription().ConfigureAwait(false);
                }
                _semaphore.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// This method cancels the current call of DetectChangesAsync. After calling the DetectChangesAsync again,
        /// this cancellation is automatically reseted and ready for a newly cancellation.
        /// </summary>
        public void CancelCurrentDetection()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// Add items to the subscription. This items are active in the next watch cycle, so you will get a data change if the update was activated.
        /// </summary>
        /// <param name="vars">Vars to activate </param>
        public Task<bool> AddItemsAsync(params PlcWatchReference[] vars) => AddItemsAsync(vars as IEnumerable<PlcWatchReference>);

        /// <summary>
        /// Add items to the subscription. This items are active in the next watch cycle, so you will get a data change if the update was activated.
        /// </summary>
        /// <param name="vars">Vars to activate </param>
        public Task<bool> TryAddItemsAsync(params PlcWatchReference[] vars) => TryAddItemsAsync(vars as IEnumerable<PlcWatchReference>);

        /// <summary>
        /// Add items to the subscription. This items are active in the next watch cycle, so you will get a data change if the update was activated.
        /// </summary>
        /// <param name="vars">Vars to activate </param>
        public Task<bool> AddItemsAsync(IEnumerable<PlcWatchReference> vars) => InternalAddItemsAsync(vars, true);

        /// <summary>
        /// Add items to the subscription. This items are active in the next watch cycle, so you will get a data change if the update was activated.
        /// </summary>
        /// <param name="vars">Vars to activate </param>
        public Task<bool> TryAddItemsAsync(IEnumerable<PlcWatchReference> vars) => InternalAddItemsAsync(vars, false);


        /// <summary>
        /// Remove items from the watch list. This items will be removed before the next what cycle.
        /// The internal 
        /// </summary>
        /// <param name="vars"></param>
        public Task<bool> RemoveItemsAsync(params PlcWatchReference[] vars) => RemoveItemsAsync(vars as IEnumerable<PlcWatchReference>);

        /// <summary>
        /// Remove items from the watch list. This items will be removed before the next watch cycle.
        /// The internal 
        /// </summary>
        /// <param name="vars"></param>
        public async Task<bool> RemoveItemsAsync(IEnumerable<PlcWatchReference> vars)
        {
            var filteredVars = vars.Where(item => !string.IsNullOrEmpty(item.Address)).ToList();
            using (await SemaphoreGuard.Async(_semaphore).ConfigureAwait(false))
            {
                var result = false;
                foreach (var item in filteredVars)
                {
                    if(_variables.Remove(item.Address))
                    {
                        result = true;
                    }
                }
                if (result)
                {
                    UpdateWatchCycle(_variables.Values);
                    _modified = true;
                    _modifiedEvent?.Set(true);
                }
                return result;
            }
        }

        /// <summary>
        /// Check if the given reference is already in this subscription..
        /// </summary>
        /// <param name="plcWatchReference"><see cref="PlcWatchReference"/></param>
        /// <returns> returns true if the same item (address and watchCycle) exist in the subscription.</returns>
        public bool HasItem(PlcWatchReference plcWatchReference)
        {
            if (_variables.TryGetValue(plcWatchReference.Address, out var reference))
            {
                return plcWatchReference.WatchCycle == reference.WatchCycle;
            }
            return false;
        }


        /// <summary>
        /// Returns the plc read result form the subscriptions cache
        /// </summary>
        /// <param name="vars"></param>
        /// <returns></returns>
        public async ValueTask<PlcReadResult[]?> ReadResultsFromCache(IEnumerable<PlcWatchReference> vars)
        {
            if (_executions!.IsNullOrEmpty() || !vars.Any())
            {
                return null;
            }

            List<Execution>? executions = null;
            using (await SemaphoreGuard.Async(_semaphore).ConfigureAwait(false))
            {
                executions = _executions?.ToList();
            }

            if (executions == null || executions.IsNullOrEmpty())
            {
                return null;
            }
            var variables = vars.Select(x => x.Address).ToList();
            return _mapper.Engine.ReadVariablesFromExecutionCache(variables, executions);
        }



        /// <summary>
        /// Starts a detection an returns the changes if any occurred.
        /// Also if no items are available, the detection runs as long as it will be stopper. 
        /// </summary>
        /// <returns></returns>
        public async Task<ChangeResult> DetectChangesAsync()
        {

            var cts = new CancellationTokenSource();
            if (Interlocked.CompareExchange(ref _cts, cts, null) != null)
            {
                cts.Dispose();
                ExceptionThrowHelper.ThrowMultipleDetectionsAreNotSupportedException();
            }

            try
            {
                Dictionary<string, int>? cycles = null;
                var interval = Interval;
                while (!Watching.IsCompleted)
                {
                    if (_cts.IsCancellationRequested)
                    {
                        return new ChangeResult(null, true, Watching.IsCompleted);
                    }

                    if (_modified)
                    {
                        // test if sema is free, if not test it the next cycle
                        if (_semaphore.Wait(0))
                        {
                            try
                            {
                                if (_modified)
                                {
                                    cycles = _variables.Values.ToDictionary(x => x.Address, x => x.WatchCycle);
                                    interval = Interval;
                                    _executions = _mapper.Engine.DetermineExecutions(_variables.Values);
                                    if (_changeDetectionStrategy == ChangeDetectionStrategy.Event)
                                    {
                                        var needUpdate = _mapper.Engine.UpdateableItems(_executions, false);
                                        await _mapper.UpdateMonitoringItemsAsync(needUpdate.Values).ConfigureAwait(false);
                                    }
                                    _modified = false;
                                }
                            }
                            finally
                            {
                                _semaphore.Release();
                            }
                        }
                    }

                    if (_executions == null)
                    {
                        _modified = true;
                        continue;
                    }

                    PlcReadResult[]? readRes = null;
                    var detect = DateTime.Now;
                    if (_executions.Any())
                    {
                        if (_changeDetectionStrategy == ChangeDetectionStrategy.Polling || _changeEvent == null)
                        {
                            readRes = await _mapper.Engine.ReadExecutionsAsync(_executions, true, _lastRun, DetermineChanges(cycles, interval), (x) => FilterChanged(detect, x)).ConfigureAwait(false);
                        }
                        else
                        {
                            var packs = await _changeEvent.WaitAsync(_cts.Token).ConfigureAwait(false);
                            if (packs?.Any() == true)
                            {
                                readRes = _mapper.Engine.CreatePlcReadResults(_executions, packs);
                            }
                        }

                        if (readRes?.Any() == true)
                        {
                            _lastRun = detect;
                            return new ChangeResult(readRes, Watching.IsCanceled, Watching.IsCompleted);
                        }
                    }

                    if (_cts.IsCancellationRequested)
                    {
                        return new ChangeResult(null, true, Watching.IsCompleted);
                    }

                    try
                    {
                        using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(interval)))
                        {
                            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, timeoutCts.Token))
                            {
                                await _modifiedEvent.WaitAsync(linkedCts.Token).ConfigureAwait(false);
                            }
                        }
                    }
                    catch (TaskCanceledException) 
                    { 
                    }

                    if (_cts.IsCancellationRequested)
                    {
                        return new ChangeResult(null, true, Watching.IsCompleted);
                    }
                }
                return new ChangeResult(null, Watching.IsCanceled, Watching.IsCompleted);
            }
            finally
            {
                _cts = null;
                cts?.Dispose();
            }
        }

        internal void OnDataChanged(IEnumerable<DataPack> changed)
        {
            if (_changeEvent != null)
            {
                _changeEvent.Set(changed);
            }
            else
            {
                ExceptionThrowHelper.ThrowOperationNotAllowedForCurrentChangeDetectionStrategy();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cycles">the cycle interval of the current variable</param>
        /// <param name="cycleInterval">the minimum of all watches</param>
        /// <returns></returns>
        private static Func<IEnumerable<string>, DateTime, bool> DetermineChanges(Dictionary<string, int>? cycles, int cycleInterval)
            => (variables, lastChange) => cycles != null &&
                                          !cycles.Any() &&
                                          !string.IsNullOrWhiteSpace(variables.FirstOrDefault(x => cycles.TryGetValue(x, out var interval) &&
                                                                                                   lastChange.AddMilliseconds(interval < cycleInterval * 2 ? cycleInterval : interval) < DateTime.Now));




        private async Task InternalStopSubscription()
        {
            _watchingTcs.TrySetResult(null);
            await _watchingTcs.Task.ConfigureAwait(false);
            _mapper.RemoveSubscription(this);
            _variables.Clear();
            _lruCache.Dispose();
        }


        private async Task<bool> InternalAddItemsAsync(IEnumerable<PlcWatchReference>? vars, bool throwExceptions)
        {
            if (TryBuildAddableVariableList(vars, out var variables, throwExceptions))
            {
                using (await SemaphoreGuard.Async(_semaphore).ConfigureAwait(false))
                {
                    return AddValidatedVariables(variables!);
                }
            }
            return false;
        }

        private bool TryBuildAddableVariableList(IEnumerable<PlcWatchReference>? vars, out Dictionary<string, PlcWatchReference>? variables,  bool throwExceptions)
        {
            if (vars == null || !vars.Any())
            {
                variables = null;
                return false;
            }

            variables = [];
            foreach (var variable in vars)
            {
                if (!_mapper.IsValidReference(variable))
                {
                    if (throwExceptions)
                    {
                        ExceptionThrowHelper.ThrowInvalidVariableException(variable.Address);
                    }
                    return false;
                }
                variables.Add(variable.Address, variable);
            }
            return variables.Any();
        }

        private bool AddValidatedVariables(Dictionary<string, PlcWatchReference> variables)
        {
            // After we validate all items, we add all
            // if one is not valid non of them will be added, this is because of consistence
            foreach (var variable in variables.Values)
            {
                if (_variables.ContainsKey(variable.Address))
                {
                    _variables[variable.Address] = variable;
                }
                else
                {
                    _variables.Add(variable.Address, variable);
                }
            }
            UpdateWatchCycle(_variables.Values);
            var result = _modified = true;
            _modifiedEvent?.Set(true);
            return result;
        }


        /// <summary>
        /// This filter detects which items are changed during the sleep  phase of the subscription and returns only the changed ones.
        /// </summary>
        /// <param name="detect">The timestamps of the start of the current detection run. Because we need a single time for filtering not DateTime.Now</param>
        /// <param name="all">All the items updated from the plc.</param>
        /// <returns>A all items where we detect a data change.</returns>
        private List<KeyValuePair<string, PlcObjectBinding>> FilterChanged(DateTime detect, IEnumerable<KeyValuePair<string, PlcObjectBinding>> all)
        {
            var result = new List<KeyValuePair<string, PlcObjectBinding>>();
            foreach (var binding in all)
            {
                var objBinding = binding.Value;
                if (objBinding.Data is Memory<byte> memData)
                {
                    // only work on valid items
                    if (!memData.IsEmpty)
                    {
                        var size = objBinding.Size == 0 ? 1 : objBinding.Size;
                        if (!_lruCache.TryGetValue(binding.Key, out var saved) ||
                            (objBinding.Size == 0
                                ? memData.Span[objBinding.Offset].GetBit(objBinding.MetaData.Offset.Bits) != saved.Data.Span[0].GetBit(objBinding.MetaData.Offset.Bits)
                                : !memData.Slice(objBinding.Offset, size).Span.SequenceEqual(saved.Data[..size].Span)))
                        {
                            DebugOutPut("Subscription {0}: Change detected on {1}", Id, binding.Key);
                            result.Add(binding);
                            var data = memData.Slice(objBinding.Offset, size);
                            if (saved == null)
                            {
                                _lruCache.Create(binding.Key, data, detect, objBinding.ValidationTimeInMs);
                            }
                            else
                            {
                                LruCache.Update(saved, data, detect);
                            }
                        }
                        else
                        {
                            LruCache.Update(saved, detect);
                        }
                    }
                }
                else
                {
                    if (objBinding.Data is object data)
                    {
                        if (!_lruCache.TryGetValue(binding.Key, out var saved) || !data.Equals(saved.DataObject))
                        {
                            DebugOutPut("Subscription {0}: Change detected on object {1}", Id, binding.Key);
                            result.Add(binding);
                            if (saved == null)
                            {
                                _lruCache.Create(binding.Key, data, detect, objBinding.ValidationTimeInMs);
                            }
                            else
                            {
                                LruCache.Update(saved, data, detect);
                            }
                        }
                        else
                        {
                            LruCache.Update(saved, detect);
                        }
                    }
                }
            }

            _lruCache.RemoveUnused(detect);

            return result;
        }


        private void UpdateWatchCycle(IEnumerable<PlcWatchReference>? vars)
        {
            var hasItems = vars != null && vars.Any();
            if (hasItems)
            {
                Interval = vars!.Select(x => x.WatchCycle).Min();
            }
            if (Interval <= 0 || (!hasItems && Interval < _defaultInterval))
            {
                Interval = _defaultInterval;
            }
        }


        [Conditional("DEBUG")]
        private static void DebugOutPut(string format, params object[] attributes) => Debug.WriteLine(format, attributes);
    }
}