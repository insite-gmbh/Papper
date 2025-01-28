using Papper.Extensions.Metadata;
using Papper.Extensions.Notification;
using Papper.Tests.Mappings;
using Papper.Tests.Mappings.ChargenRV2;
using Papper.Tests.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Papper.Tests
{
    public sealed class SubscriptionTests : IDisposable
    {
        private static readonly MockPlc _mockPlc = new();
        private readonly PlcDataMapper _papper = new(960, Papper_OnRead, Papper_OnWrite);
        private readonly ITestOutputHelper _output;

        public SubscriptionTests(ITestOutputHelper output)
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
                    { "W88", (ushort)3},
                    { "X99.0", true  },
                };
            var items = writeData.Keys.Select(variable => PlcWatchReference.FromAddress($"DB115.{variable}", 100)).ToArray();

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
            var mapping = "DB_SafetyDataChange1";
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
        public void StartSubscriptionWithOutVariablesTest()
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
                    { "W88", (ushort)3},
                    { "X99_0", true  },
                };
            var items = writeData.Keys.Select(variable => PlcReadReference.FromAddress($"DB116.{variable}")).ToArray();

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
                    { "W88", (ushort)3},
                    { "X99_0", true  },
                };
            var items = writeData.Keys.Select(variable => PlcWatchReference.FromAddress($"DB117.{variable}", 100)).ToArray();

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
        public void DataChangeTest()
        {
            var sleepTime = 10000;
            var mapping = "DB_SafetyDataChange1";
            var intiState = true;
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
            using (var are = new AutoResetEvent(false))
            {
                using (var subscription = _papper.CreateSubscription())
                {
                    subscription.AddItemsAsync(originData.Keys.Select(variable => PlcWatchReference.FromAddress($"{mapping}.{variable}", 100)));
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
        public void TestBitDataChange()
        {
            var sleepTime = 10000;
            var address = "DB_SafetyDataChange.SafeMotion.Slots[15].Commands.TakeoverPermitted";
            var are = new AutoResetEvent(false);
            var changes = 0;

            using (var subscription = _papper.CreateSubscription())
            {
                subscription.AddItemsAsync(PlcWatchReference.FromAddress(address, 100));
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








        [Fact]
        public async Task PerformRawDataChange()
        {
            var intiState = true;
            var originData = new Dictionary<string, object> {
                    { "W88", (ushort)0},
                    { "X99_0", false  },
                    { "DW100", (uint)0},
                };
            var writeData = new Dictionary<string, object> {
                    { "W88", (ushort)3},
                    { "X99_0", true  },
                    { "DW100", (uint)5},
                };
            using var are = new AutoResetEvent(false);
            void callback(object s, PlcNotificationEventArgs e)
            {
                foreach (var item in e)
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
                are.Set();
            }
            var subscription = _papper.SubscribeDataChanges(callback, writeData.Keys.Select(variable => PlcWatchReference.FromAddress($"DB15.{variable}", 100)).ToArray());


            //waiting for initialize
            Assert.True(are.WaitOne(5000));
            intiState = false;
            var writeResults = await _papper.WriteAsync(PlcWriteReference.FromRoot("DB15", writeData.ToArray()).ToArray()).ConfigureAwait(false);
            foreach (var item in writeResults)
            {
                Assert.Equal(ExecutionResult.Ok, item.ActionResult);
            }

            //waiting for write update
            Assert.True(are.WaitOne(5000));

            //test if data change only occurred if data changed
            Assert.False(are.WaitOne(5000));

            await subscription.DisposeAsync();

           
        }



        [Fact]
        public async Task PerformRawDataArrayChange()
        {
            var intiState = true;
            var originData = new Dictionary<string, object> {
                    { "B0_10", new byte[]{ 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } },
                };
            var writeData = new Dictionary<string, object> {
                    { "B0_10", new byte[]{ 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 } },
                };
            using var are = new AutoResetEvent(false);
            void callback(object s, PlcNotificationEventArgs e)
            {

                if(e.Exception != null)
                {

                }


                foreach (var item in e)
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
                are.Set();
            }
            var subscription = _papper.SubscribeDataChanges(callback, writeData.Keys.Select(variable => PlcWatchReference.FromAddress($"DB66.{variable}", 100)).ToArray());


            //waiting for initialize
            Assert.True(are.WaitOne(50000));
            intiState = false;
            var writeResults = await _papper.WriteAsync(PlcWriteReference.FromRoot("DB66", writeData.ToArray()).ToArray()).ConfigureAwait(false);
            foreach (var item in writeResults)
            {
                Assert.Equal(ExecutionResult.Ok, item.ActionResult);
            }

            //waiting for write update
            Assert.True(are.WaitOne(5000));

            //test if data change only occurred if data changed
            Assert.False(are.WaitOne(5000));

            await subscription.DisposeAsync();


        }


        [Fact]
        public void TestExternalDataChange()
        {

            using var papper = new PlcDataMapper(960, Papper_OnRead, Papper_OnWrite, UpdateHandler, ReadMetaData, OptimizerType.Items);
            papper.AddMapping(typeof(DB_Safety));
            _mockPlc.OnItemChanged = (items) =>
            {
                papper.OnDataChanges(items.Select(i => new DataPackAbsolute
                {
                    Selector = i.Selector,
                    Offset = i.Offset,
                    Length = i.Length,
                    BitMaskBegin = i.BitMaskBegin,
                    BitMaskEnd = i.BitMaskEnd
                }.ApplyResult(ExecutionResult.Ok, i.Data)));
            };
            var sleepTime = 10000;
            var mapping = "DB_Safety";
            var intiState = true;
            var originData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[16].SlotId", (byte)0},
                    { "SafeMotion.Slots[16].HmiId", (uint)0},
                    { "SafeMotion.Slots[16].Commands.TakeoverPermitted", false },
                };
            var writeData = new Dictionary<string, object> {
                    { "SafeMotion.Slots[16].SlotId", (byte)3},
                    { "SafeMotion.Slots[16].HmiId", (uint)4},
                    { "SafeMotion.Slots[16].Commands.TakeoverPermitted", false },
                };
            using var are = new AutoResetEvent(false);

            // write initial state
            papper.WriteAsync(PlcWriteReference.FromRoot(mapping, originData.ToArray()).ToArray()).GetAwaiter().GetResult();

            using (var subscription = papper.CreateSubscription(ChangeDetectionStrategy.Event))
            {
                subscription.AddItemsAsync(originData.Keys.Select(variable => PlcWatchReference.FromAddress($"{mapping}.{variable}", 100)));
                var t = Task.Run(async () =>
                {
                    try
                    {
                        while (!subscription.Watching.IsCompleted)
                        {
                            var res = await subscription.DetectChangesAsync().ConfigureAwait(false);

                            if (!res.IsCompleted && !res.IsCanceled)
                            {
                                _output.WriteLine($"Changed: initial state is {intiState}");
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
                                        _output.WriteLine($"Changed: {item.Variable} = {item.Value}");

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
                Assert.True(are.WaitOne(sleepTime), "waiting for initialize");

                intiState = false;
                var writeResults = papper.WriteAsync(PlcWriteReference.FromRoot(mapping, writeData.ToArray()).ToArray()).GetAwaiter().GetResult();
                foreach (var item in writeResults)
                {
                    Assert.Equal(ExecutionResult.Ok, item.ActionResult);
                }
                //waiting for write update
                Assert.True(are.WaitOne(sleepTime), "waiting for write update");

                //test if data change only occurred if data changed
                Assert.False(are.WaitOne(sleepTime), $"test if data change only occurred if data changed");

            }

           
        }




        [Fact]
        public async Task TestMultipleChanges()
        {
            var address1 = "DB_SafetyDataChange.SafeMotion.Slots[11].Commands.TakeoverPermitted";
            var address2 = "DB_SafetyDataChange.SafeMotion.Slots[11].Commands";
            var address1Changes = 0;
            var address2Changes = 0;
            var t1 = Task.Run(async () =>
           {
               using (var subscription = _papper.CreateSubscription())
               {
                   await subscription.AddItemsAsync(PlcWatchReference.FromAddress(address1, 50)).ConfigureAwait(false);
                   try
                   {
                       while (!subscription.Watching.IsCompleted)
                       {
                           _output.WriteLine($"1:DetectChangesAsync");
                           var res = await subscription.DetectChangesAsync().ConfigureAwait(false);
                           _output.WriteLine($"1:DetectChangesAsync has a result");
                           if (!res.IsCompleted && !res.IsCanceled)
                           {
                               var resItem = res.Results.FirstOrDefault();
                               _output.WriteLine($"1: {resItem.Address}={resItem.Value}");
                               address1Changes++;
                           }
                       }
                   }
                   catch (Exception ex)
                   {
                       _output.WriteLine($"1 Error: {ex.Message}");
                   }
                   _output.WriteLine($"1 Finished");
               }
           });


            var t2 = Task.Run(async () =>
            {
                using (var subscription = _papper.CreateSubscription())
                {
                    await subscription.AddItemsAsync(PlcWatchReference.FromAddress(address2, 50)).ConfigureAwait(false);
                    try
                    {
                        while (!subscription.Watching.IsCompleted)
                        {
                            _output.WriteLine($"2:DetectChangesAsync");
                            var res = await subscription.DetectChangesAsync().ConfigureAwait(false);
                            _output.WriteLine($"2:DetectChangesAsync has a result");
                            if (!res.IsCompleted && !res.IsCanceled)
                            {
                                var resItem = res.Results.FirstOrDefault();
                                _output.WriteLine($"2: {resItem.Address}={resItem.Value}");
                                address2Changes++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"2 Error: {ex.Message}");
                    }
                    _output.WriteLine($"2 Finished");
                }
            });


            await Task.Delay(1000).ConfigureAwait(false);
            var exec1 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress(address1, 200) });
            _output.WriteLine($"Exec1: {exec1[0].PlcRawData.Size}");
            var exec2 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress(address2, 200) });
            _output.WriteLine($"Exec2: {exec2[0].PlcRawData.Size}");

            for (var i = 0; i < 5; i++)
            {
                _output.WriteLine($"WriteAsync: {i % 2 == 0}");
                var writeResults = await _papper.WriteAsync(PlcWriteReference.FromAddress(address1, i % 2 == 0)).ConfigureAwait(false);
                
                foreach (var item in writeResults)
                {
                    Assert.Equal(ExecutionResult.Ok, item.ActionResult);
                    _output.WriteLine($"WriteAsync: {item.ActionResult}");
                }
                await Task.Delay(250).ConfigureAwait(false);
            }

            await Task.Delay(1000).ConfigureAwait(false);


            Assert.Equal(address1Changes, address2Changes);
            Assert.Equal(6, address1Changes);
            Assert.Equal(6, address2Changes);

        }

        [Fact]
        public async Task MultipleSubscriptions()
        {
            string[] address = ["DB260.X16310.1", "DB260.S16288,20", "DB260.C16046,2", "DB260.X16052.1", "DB260.X16052.0", "DB260.X16052.2", "DB260.X14.0", "DB260.X14.1", "DB260.X14.2", "DB260.X14.3", "DB260.X14.4", "DB260.X14.5"]; // "DB_SafetyDataChange.SafeMotion.Slots[11].Commands.TakeoverPermitted";
            var watchItems = address.Select(address => PlcWatchReference.FromAddress(address, 50)).ToList();
            var address1Changes = 0;
            var address2Changes = 0;
            var t1 = Task.Run(async () =>
            {
                using (var subscription = _papper.CreateSubscription())
                {
                    await subscription.AddItemsAsync(watchItems).ConfigureAwait(false);
                    try
                    {
                        while (!subscription.Watching.IsCompleted)
                        {
                            _output.WriteLine($"1:DetectChangesAsync");
                            var res = await subscription.DetectChangesAsync().ConfigureAwait(false);
                            _output.WriteLine($"1:DetectChangesAsync has a result");
                            if (!res.IsCompleted && !res.IsCanceled)
                            {
                                var resItem = res.Results.FirstOrDefault();
                                _output.WriteLine($"1: {resItem.Address}={resItem.Value}");
                                address1Changes++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"1 Error: {ex.Message}");
                    }
                    _output.WriteLine($"1 Finished");
                }
            });


            var t2 = Task.Run(async () =>
            {
                using (var subscription = _papper.CreateSubscription())
                {
                    await subscription.AddItemsAsync(watchItems).ConfigureAwait(false);
                    try
                    {
                        while (!subscription.Watching.IsCompleted)
                        {
                            _output.WriteLine($"2:DetectChangesAsync");
                            var res = await subscription.DetectChangesAsync().ConfigureAwait(false);
                            _output.WriteLine($"2:DetectChangesAsync has a result");
                            if (!res.IsCompleted && !res.IsCanceled)
                            {
                                var resItem = res.Results.FirstOrDefault();
                                _output.WriteLine($"2: {resItem.Address}={resItem.Value}");
                                address2Changes++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"2 Error: {ex.Message}");
                    }
                    _output.WriteLine($"2 Finished");
                }
            });


            await Task.Delay(1000).ConfigureAwait(false);
            var exec1 = _papper.Engine.DetermineExecutions(watchItems);
            _output.WriteLine($"Exec1: {exec1[0].PlcRawData.Size}");
            var exec2 = _papper.Engine.DetermineExecutions(watchItems);
            _output.WriteLine($"Exec2: {exec2[0].PlcRawData.Size}");

            for (var i = 0; i < 5; i++)
            {
                _output.WriteLine($"WriteAsync: {i % 2 == 0}");
                var writeResults = await _papper.WriteAsync(PlcWriteReference.FromAddress(address[0], i % 2 == 0)).ConfigureAwait(false);

                foreach (var item in writeResults)
                {
                    Assert.Equal(ExecutionResult.Ok, item.ActionResult);
                    _output.WriteLine($"WriteAsync: {item.ActionResult}");
                }
                await Task.Delay(200).ConfigureAwait(false);
            }

            await Task.Delay(1000).ConfigureAwait(false);


            Assert.Equal(address1Changes, address2Changes);
            Assert.Equal(6, address1Changes);
            Assert.Equal(6, address2Changes);


        }




        private static Task Papper_OnRead(IEnumerable<DataPack> reads)
        {
            var result = reads.ToList();
            foreach (var item in result.OfType<DataPackAbsolute>())
            {
                //Console.WriteLine($"OnRead: selector:{item.Selector}; offset:{item.Offset}; length:{item.Length}");
                var res = _mockPlc.GetPlcEntry(item.Selector, item.Offset + item.Length).Data.Slice(item.Offset, item.Length);
                if (!res.IsEmpty)
                {
                    var data = new byte[res.Length];
                    res.CopyTo(data);
                    item.ApplyResult(ExecutionResult.Ok, data);
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
            foreach (var item in result.OfType<DataPackAbsolute>())
            {
                var entry = _mockPlc.GetPlcEntry(item.Selector, item.Offset + item.Length);
                if (!item.HasBitMask)
                {
                    //Console.WriteLine($"OnWrite: selector:{item.Selector}; offset:{item.Offset}; length:{item.Length}");
                    item.Data.Slice(0, item.Length).CopyTo(entry.Data.Slice(item.Offset, item.Length));
                    item.ApplyResult(ExecutionResult.Ok);
                }
                else
                {
                    var lastItem = item.Data.Length - 1;
                    for (var j = 0; j < item.Data.Length; j++)
                    {
                        var bItem = item.Data.Span[j];
                        if (j > 0 && j < lastItem)
                        {
                            entry.Data.Span[item.Offset + j] = item.Data.Span[j];
                            item.ApplyResult(ExecutionResult.Ok);
                        }
                        else
                        {
                            var bm = j == 0 ? item.BitMaskBegin : (j == lastItem) ? item.BitMaskEnd : (byte)0;
                            if (bm == 0xFF)
                            {
                                entry.Data.Span[item.Offset + j] = item.Data.Span[j];
                                item.ApplyResult(ExecutionResult.Ok);
                            }
                            else if (bm > 0)
                            {
                                for (var i = 0; i < 8; i++)
                                {
                                    var bit = bm.GetBit(i);
                                    if (bit)
                                    {
                                        var b = entry.Data.Span[item.Offset + j];
                                        entry.Data.Span[item.Offset + j] = b.SetBit(i, bItem.GetBit(i));
                                        item.ApplyResult(ExecutionResult.Ok);
                                        bm = bm.SetBit(i, false);
                                        if (bm == 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }


        private Task UpdateHandler(IEnumerable<DataPack> monitoring, bool add = true)
        {
            foreach (var item in monitoring)
            {
                _mockPlc.UpdateDataChangeItem(item, !add);
            }
            return Task.CompletedTask;
        }

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
