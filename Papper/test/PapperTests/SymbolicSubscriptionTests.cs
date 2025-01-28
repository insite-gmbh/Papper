using Papper.Extensions.Metadata;
using Papper.Extensions.Notification;
using Papper.Tests.Mappings;
using Papper.Tests.Mappings.ChargenRV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Papper.Tests
{

    [Collection("Sequential")]
    public sealed class SymbolicSubscriptionTests : IDisposable
    {
        private static readonly Dictionary<string, object> _mockPlc = new();
        private readonly PlcDataMapper _papper = new(1000000, Papper_OnRead, Papper_OnWrite, OptimizerType.Symbolic);
        private readonly ITestOutputHelper _output;

        public SymbolicSubscriptionTests(ITestOutputHelper output)
        {
            _output = output;
            _papper.AddMapping(typeof(DB_Safety));
            _papper.AddMapping(typeof(ArrayTestMapping));
            _papper.AddMapping(typeof(StringArrayTestMapping));
            _papper.AddMapping(typeof(PrimitiveValuesMapping));
            _papper.AddMapping(typeof(DB_MotionHMI));
            _papper.AddMapping(typeof(DB_BST1_ChargenRV));
            _papper.AddMapping(typeof(MSpindleInterface));
        }


        [Fact]
        public async Task AddAndRemoveSubscriptionsTest()
        {
            var writeData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[15].SlotId", (byte)0},
                    { "SafeMotion.Slots[15].HmiId", (uint)0}
                };
            await _papper.WriteAsync(writeData.Select( c =>  PlcWriteReference.FromAddress($"DB_SafetyDataChange2.{c.Key}", c.Value))).ConfigureAwait(false);
            var items = writeData.Keys.Select(variable => PlcWatchReference.FromAddress($"DB_SafetyDataChange2.{variable}", 100)).ToArray();

            using (var sub = _papper.CreateSubscription())
            {
                Assert.False(sub.HasVariables);
                Assert.True(await sub.TryAddItemsAsync(items).ConfigureAwait(false));
                var c = sub.DetectChangesAsync();  // returns because we start a new detection
               
                Assert.True(sub.HasVariables, "has no variable");
                Assert.Equal(2, sub.Count);
                Assert.True(await sub.RemoveItemsAsync(items.FirstOrDefault()).ConfigureAwait(false), "could not remove first item");
                Assert.Equal(1, sub.Count);
                Assert.False(await sub.RemoveItemsAsync(items.FirstOrDefault()).ConfigureAwait(false), "Could remove item but has no"); // <- coudl not remove twice

                await c.ConfigureAwait(false); // wait for the other detection to complete
                c = sub.DetectChangesAsync();      // returns because we modified the detection
                Assert.False(await sub.RemoveItemsAsync(items.FirstOrDefault()).ConfigureAwait(false), "Could remove item but has no");

                Assert.True(await sub.RemoveItemsAsync(items.LastOrDefault()).ConfigureAwait(false));
                Assert.Equal(0, sub.Count);
                Assert.False(sub.HasVariables);
            }
        }

        [Fact]
        public async Task AddAndRemoveAllItemsFromSubscriptionsTest()
        {
            var mapping = "DB_SafetyDataChange2";
            var originData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[15].SlotId", (byte)0},
                    { "SafeMotion.Slots[15].HmiId", (uint)0},
                    { "SafeMotion.Slots[15].Commands.TakeoverPermitted", false },
                };
            var writeData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[15].SlotId", (byte)3},
                    { "SafeMotion.Slots[15].HmiId", (uint)4},
                    { "SafeMotion.Slots[15].Commands.TakeoverPermitted", false },
                };

            await _papper.WriteAsync(originData.Select(c => PlcWriteReference.FromAddress(c.Key, c.Value))).ConfigureAwait(false);
            using (var are = new AutoResetEvent(false))
            {
                using (var subscription = _papper.CreateSubscription())
                {
                    var items = originData.Keys.Select(variable => PlcWatchReference.FromAddress($"{mapping}.{variable}", 100)).ToArray();
                    await subscription.AddItemsAsync(items).ConfigureAwait(false);

                    Assert.True(subscription.HasVariables);


                    await subscription.RemoveItemsAsync(items).ConfigureAwait(false);

                    Assert.False(subscription.HasVariables);
                }
            }
        }


        [Fact]
        public void StartSubscriptionWithOutVaraiblesTest()
        {
            using (var sub = _papper.CreateSubscription())
            {
                Assert.False(sub.HasVariables);
                var c = sub.DetectChangesAsync();  // returns because we start a new detection
               
                Assert.False(sub.HasVariables);
            }

           
        }


        [Fact]
        public async Task DuplicateDetectionTest()
        {
            var writeData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[14].SlotId", (byte)0},
                    { "SafeMotion.Slots[14].HmiId", (uint)0},
                };
            var items = writeData.Keys.Select(variable => PlcReadReference.FromAddress($"DB_SafetyDataChange3.{variable}")).ToArray();

            using (var sub = _papper.CreateSubscription())
            {
                await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                {
                    var c1 = sub.DetectChangesAsync();  // returns because we start a new detection
                    var c2 = await sub.DetectChangesAsync().ConfigureAwait(false);  // returns because we start a new detection
                    await c1.ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task SubscriptionCancellationTest()
        {
            var writeData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[15].SlotId", (byte)0},
                    { "SafeMotion.Slots[15].HmiId", (uint)0},
                };
            var items = writeData.Keys.Select(variable => PlcWatchReference.FromAddress($"DB_SafetyDataChange4.{variable}", 100)).ToArray();
            await _papper.WriteAsync(writeData.Select(c => PlcWriteReference.FromAddress($"DB_SafetyDataChange4.{c.Key}", c.Value))).ConfigureAwait(false);
            using (var sub = _papper.CreateSubscription())
            {
                Assert.True(await sub.TryAddItemsAsync(items).ConfigureAwait(false));
                var c = sub.DetectChangesAsync();  // returns because we start a new detection
                Thread.Sleep(500);
                var res = await c.ConfigureAwait(false);
                Assert.False(res.IsCanceled);
                Assert.False(res.IsCompleted);
                Assert.NotNull(res.Results);
                Assert.Equal(2, res.Results.Count());

                c = sub.DetectChangesAsync();
                Thread.Sleep(500);
                sub.Pause();
                res = await c.ConfigureAwait(false);
                Assert.True(res.IsCanceled);
                Assert.False(res.IsCompleted);
                Assert.Null(res.Results);

            }
        }

        [Fact]
        public async Task DataChangeTest()
        {
            var sleepTime = 10000;
            var mapping = "DB_SafetyDataChange2";
            var intiState = true;
            var originData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[15].SlotId", (byte)0},
                    { "SafeMotion.Slots[15].HmiId", (uint)0},
                    { "SafeMotion.Slots[15].Commands.TakeoverPermitted", false },
                };
            await _papper.WriteAsync(originData.Select(c => PlcWriteReference.FromAddress($"{mapping}.{c.Key}", c.Value))).ConfigureAwait(false);
            var writeData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[15].SlotId", (byte)3},
                    { "SafeMotion.Slots[15].HmiId", (uint)4},
                    { "SafeMotion.Slots[15].Commands.TakeoverPermitted", false },
                };
            using (var are = new AutoResetEvent(false))
            {
                using (var subscription = _papper.CreateSubscription())
                {
                    _=  subscription.AddItemsAsync(originData.Keys.Select(variable => PlcWatchReference.FromAddress($"{mapping}.{variable}", 100)));
                    var t = Task.Run(async () =>
                    {
                        try
                        {
                            while (!subscription.Watching.IsCompleted)
                            {
                                var res = await subscription.DetectChangesAsync().ConfigureAwait(false);

                                if (!res.IsCompleted && !res.IsCanceled)
                                {
                                    if (!intiState)
                                    {
                                        Assert.Equal(2, res.Results.Count());
                                    }
                                    else
                                    {
                                        Assert.Equal(3, res.Results.Count());
                                    }

                                    foreach (var item in res.Results)
                                    {
                                        try
                                        {
                                            if (!intiState)
                                            {
                                                Assert.Equal(writeData[item.Variable], item.Value);
                                            }
                                            else
                                            {
                                                Assert.Equal(originData[item.Variable], item.Value);
                                            }
                                        }
                                        catch (Exception)
                                        {

                                        }
                                    }

                                    are.Set();
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    });

                    //waiting for initialize
                    Assert.True(are.WaitOne(sleepTime));
                    intiState = false;
                    var writeResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, writeData.ToArray()).ToArray()).GetAwaiter().GetResult();
                    foreach (var item in writeResults)
                    {
                        Assert.Equal(ExecutionResult.Ok, item.ActionResult);
                    }
                    //waiting for write update
                    Assert.True(are.WaitOne(sleepTime));

                    //test if data change only occurred if data changed
                    Assert.False(are.WaitOne(sleepTime));

                }
            }

           
        }

        [Fact]
        public async Task TestBitDataChange()
        {
            var sleepTime = 14000;
            var address = "DB_SafetyDataChange.SafeMotion.Slots[15].Commands.TakeoverPermitted";
            var are = new AutoResetEvent(false);
            var changes = 0;
            await _papper.WriteAsync(PlcWriteReference.FromAddress(address, false)).ConfigureAwait(false);
            using (var subscription = _papper.CreateSubscription())
            {
                _ = subscription.AddItemsAsync(PlcWatchReference.FromAddress(address, 100));
                var t = Task.Run(async () =>
                {
                    try
                    {
                        while (!subscription.Watching.IsCompleted)
                        {
                            var res = await subscription.DetectChangesAsync().ConfigureAwait(false);

                            if (!res.IsCompleted && !res.IsCanceled)
                            {
                                are.Set();
                                changes++;
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                });

                //waiting for initialize
                Assert.True(are.WaitOne(sleepTime));

                for (var i = 0; i < 5; i++)
                {
                    var writeResults = _papper.WriteAsync(PlcWriteReference.FromAddress(address, i % 2 == 0)).GetAwaiter().GetResult();
                    foreach (var item in writeResults)
                    {
                        Assert.Equal(ExecutionResult.Ok, item.ActionResult);
                    }
                    //waiting for write update
                    Assert.True(are.WaitOne(sleepTime));
                }


                Assert.Equal(6, changes);

                are.Dispose();
            }


           
        }









        //[Fact]
        //public void TestExternalDataChange()
        //{

        //    using var papper = new PlcDataMapper(960, Papper_OnRead, Papper_OnWrite, UpdateHandler, ReadMetaData, OptimizerType.Items);
        //    papper.AddMapping(typeof(DB_Safety));
        //    _mockPlc.OnItemChanged = (items) =>
        //    {
        //        papper.OnDataChanges(items.Select(i => new DataPackAbsolute
        //        {
        //            Selector = i.Selector,
        //            Offset = i.Offset,
        //            Length = i.Length,
        //            BitMaskBegin = i.BitMaskBegin,
        //            BitMaskEnd = i.BitMaskEnd
        //        }.ApplyResult(ExecutionResult.Ok, i.Data)));
        //    };
        //    var sleepTime = 10000;
        //    var mapping = "DB_Safety";
        //    var intiState = true;
        //    var originData = new Dictionary<string, object> {
        //            { "SafeMotion.Slots[16].SlotId", (byte)0},
        //            { "SafeMotion.Slots[16].HmiId", (uint)0},
        //            { "SafeMotion.Slots[16].Commands.TakeoverPermitted", false },
        //        };
        //    var writeData = new Dictionary<string, object> {
        //            { "SafeMotion.Slots[16].SlotId", (byte)3},
        //            { "SafeMotion.Slots[16].HmiId", (uint)4},
        //            { "SafeMotion.Slots[16].Commands.TakeoverPermitted", false },
        //        };
        //    using var are = new AutoResetEvent(false);

        //    // write initial state
        //    papper.WriteAsync(PlcWriteReference.FromRoot(mapping, originData.ToArray()).ToArray()).GetAwaiter().GetResult();

        //    using (var subscription = papper.CreateSubscription(ChangeDetectionStrategy.Event))
        //    {
        //        subscription.AddItemsAsync(originData.Keys.Select(variable => PlcWatchReference.FromAddress($"{mapping}.{variable}", 100)));
        //        var t = Task.Run(async () =>
        //        {
        //            try
        //            {
        //                while (!subscription.Watching.IsCompleted)
        //                {
        //                    var res = await subscription.DetectChangesAsync().ConfigureAwait(false);

        //                    if (!res.IsCompleted && !res.IsCanceled)
        //                    {
        //                        _output.WriteLine($"Changed: initial state is {intiState}");
        //                        if (!intiState)
        //                        {
        //                            Assert.Equal(2, res.Results.Count());
        //                        }
        //                        else
        //                        {
        //                            Assert.Equal(3, res.Results.Count());
        //                        }

        //                        foreach (var item in res.Results)
        //                        {
        //                            try
        //                            {
        //                                _output.WriteLine($"Changed: {item.Variable} = {item.Value}");

        //                                if (!intiState)
        //                                {
        //                                    Assert.Equal(writeData[item.Variable], item.Value);
        //                                }
        //                                else
        //                                {
        //                                    Assert.Equal(originData[item.Variable], item.Value);
        //                                }
        //                            }
        //                            catch (Exception)
        //                            {

        //                            }
        //                        }

        //                        are.Set();
        //                    }
        //                }
        //            }
        //            catch (Exception)
        //            {

        //            }
        //        });

        //        //waiting for initialize
        //        Assert.True(are.WaitOne(sleepTime), "waiting for initialize");

        //        intiState = false;
        //        var writeResults = papper.WriteAsync(PlcWriteReference.FromRoot(mapping, writeData.ToArray()).ToArray()).GetAwaiter().GetResult();
        //        foreach (var item in writeResults)
        //        {
        //            Assert.Equal(ExecutionResult.Ok, item.ActionResult);
        //        }
        //        //waiting for write update
        //        Assert.True(are.WaitOne(sleepTime), "waiting for write update");

        //        //test if data change only occurred if data changed
        //        Assert.False(are.WaitOne(sleepTime), $"test if data change only occurred if data changed");

        //    }

           
        //}



        private static Task Papper_OnRead(IEnumerable<DataPack> reads)
        {
            var result = reads.ToList();
            foreach (var item in result.OfType<DataPackSymbolic>())
            {
                Console.WriteLine($"OnRead: SymbolicName:{item.SymbolicName};");
                if (_mockPlc.TryGetValue(item.SymbolicName, out var val))
                {
                    item.ApplyResult(ExecutionResult.Ok, val);
                }
                else
                {
                    item.ApplyResult(ExecutionResult.Error);
                }
            }
            return Task.CompletedTask;
        }

        private static Task Papper_OnWrite(IEnumerable<DataPack> reads)
        {
            var result = reads.ToList();
            foreach (var item in result.OfType<DataPackSymbolic>())
            {
                _mockPlc[item.SymbolicName] = item.Value;
                item.ApplyResult(ExecutionResult.Ok);
            }
            return Task.CompletedTask;
        }


        //private Task UpdateHandler(IEnumerable<DataPack> monitoring, bool add = true)
        //{
        //    foreach (var item in monitoring)
        //    {
        //        _mockPlc[] = !add;
        //    }
        //    return Task.CompletedTask;
        //}

        private Task ReadMetaData(IEnumerable<MetaDataPack> packs)
        {
            foreach (var item in packs)
            {
                item.ExecutionResult = ExecutionResult.Error;
            }
            return Task.CompletedTask;
        }
        public void Dispose() => _papper?.Dispose();
    }
}
