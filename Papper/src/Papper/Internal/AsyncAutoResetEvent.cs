using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Papper.Internal
{
    internal class AsyncAutoResetEvent<T>
    {
        private static readonly Task<T?> _completed = Task.FromResult<T?>(default);
        private readonly Queue<TaskCompletionSource<T?>> _waits = new();
        private bool _signaled;
        private T? _lastValue = default;


        /// <summary>
        /// Waits asynchronous of an event or the given timeout.
        /// </summary>
        /// <param name="timeout">Maximum time to wait  (-1 = endless wait)</param>
        /// <returns>The value which was applied on calling set.</returns>
        public Task<T?> WaitAsync(int timeout = -1)
        {
            if (timeout == 0)
            {
                return WaitAsync(new CancellationToken(timeout == 0));
            }
            else if (timeout > -1)
            {
                var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeout));
                return WaitAsync(cts.Token).ContinueWith(t =>
                {
                    if (cts != null)
                    {
                        cts.Dispose();
                    }
                    return t.Result;
                });
            }
            return WaitAsync(CancellationToken.None);
        }


        /// <summary>
        /// Waits asynchronous of an event or the when it will be canceled by the <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="token">The waiting phase can be canceled by the source of the given token.</param>
        /// <returns>The value which was applied on calling set.</returns>
        public Task<T?> WaitAsync(CancellationToken token)
        {
            lock (_waits)
            {
                if (_signaled)
                {
                    var result = Task.FromResult(_lastValue);
                    _lastValue = default;
                    _signaled = false;
                    return result;
                }
                else if (token.IsCancellationRequested)
                {
                    return _completed;
                }
                else
                {
                    CancellationTokenRegistration registration = default;
                    var tcs = new TaskCompletionSource<T?>(TaskCreationOptions.RunContinuationsAsynchronously);
                    if (token != CancellationToken.None && token.CanBeCanceled)
                    {
                        registration = token.Register(() =>
                        {
                            bool removed;
                            lock (_waits)
                            {
                                removed = _waits.RemoveMidQueue(tcs);
                            }

                            if (removed)
                            {
                                tcs.TrySetResult(default);
                            }
                        }, false);
                    }

                    _waits.Enqueue(tcs);
                    return tcs.Task.ContinueWith<T?>(t =>
                    {
                        if (token != CancellationToken.None && token.CanBeCanceled)
                        {
                            registration.Dispose();
                        }
                        return t.Result;
                    }, CancellationToken.None, TaskContinuationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);
                }
            }
        }


        /// <summary>
        /// Set a value for the Event.
        /// </summary>
        /// <param name="value">Value to set</param>
        /// <returns>true if the value could be set.</returns>
        public bool Set(T value)
        {
            TaskCompletionSource<T?>? toRelease = null;
            lock (_waits)
            {
                if (_waits.Count > 0)
                {
                    toRelease = _waits.Dequeue();
                }
                else if (!_signaled)
                {
                    _signaled = true;
                    _lastValue = value;
                }
                else
                {
                    // Could not set because it is already set.
                    return false;
                }
            }

            toRelease?.TrySetResult(value);

            return true;
        }
    }
}
