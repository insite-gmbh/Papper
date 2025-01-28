
using Insite.Customer.Data.DI_PGS_BST1;
using Papper.Attributes;
using Papper.Extensions.Metadata;
using Papper.Extensions.Notification;
using Papper.Tests.Mappings;
using Papper.Tests.Mappings.AGV;
using Papper.Tests.Mappings.BstAbw;
using Papper.Tests.Mappings.ChargenRV;
using Papper.Tests.Mappings.ChargenRV2;
using Papper.Tests.Mappings.DeviceConfig;
using Papper.Tests.Mappings.Regal;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Papper.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public sealed class PlcDataMapperSymbolicTests : IDisposable
    {
        private readonly PlcDataMapper _papper = new(960, Papper_OnRead, Papper_OnWrite, OptimizerType.Symbolic);
        private static readonly Dictionary<string, object> _mockPlc = new();

        private readonly ITestOutputHelper _output;

        public PlcDataMapperSymbolicTests(ITestOutputHelper output)
        {
            _output = output;
            _papper.AddMapping(typeof(DB_BST3_ChargenRV));
            _papper.AddMapping(typeof(DB_BST1_ChargenRV));
            _papper.AddMapping(typeof(DB_Setup_AGV_BST1));
            _papper.AddMapping(typeof(DB_SpindlePos_BST1));
            _papper.AddMapping(typeof(DB_IPSC_Konfig));
            _papper.AddMapping(typeof(DB_BST4_Boxen_1_Konfig));
            _papper.AddMapping(typeof(DB_BST1_Regal_1_Konfig));
            _papper.AddMapping(typeof(SampleData));
            _papper.AddMapping(typeof(SampleDataSubstruct));
            _papper.AddMapping(typeof(DB_Safety));
            _papper.AddMapping(typeof(ArrayTestMapping));
            _papper.AddMapping(typeof(StringArrayTestMapping));
            _papper.AddMapping(typeof(PrimitiveValuesMapping));
            _papper.AddMapping(typeof(DB_MotionHMI));
            _papper.AddMapping(typeof(MSpindleInterface));
            _papper.AddMapping(typeof(RfData));
            _papper.AddMapping(typeof(DB_BST_An_Abwahl_BST1));
            _papper.AddMapping(typeof(DB_BST1_Geraete_1_Konfig));
            _papper.AddMapping(typeof(SampleDataAccessNames));
            _papper.AddMapping(typeof(DB_TestCTT));
            _papper.AddMapping(typeof(DB_ZK_Storage_BandG));
            _papper.AddMapping(typeof(DB_Setting_BST1));
            _papper.AddMapping(typeof(DI_PGS_BST1));

        }


        [Fact]
        public async Task AddMappingTest()
        {
            _papper.AddMapping(typeof(FBSample));
            IEnumerable<string> vars = _papper.GetVariablesOf("MAIN:fbSample1");
            PlcReadResult[] result2 = await _papper.ReadAsync(PlcReadReference.FromAddress("MAIN:fbSample1"));
        }

        [Theory]
        [InlineData(nameof(DB_Setup_AGV_BST1), 4244)]
        [InlineData(nameof(DB_Safety), 4596)]
        [InlineData("ARRAY_TEST_MAPPING_1", 105030)]
        [InlineData("STRING_ARRAY_TEST_MAPPING", 21)]
        [InlineData(nameof(PrimitiveValuesMapping), 6)]
        [InlineData(nameof(DB_MotionHMI), 725)]
        [InlineData(nameof(DB_BST1_ChargenRV), 4384)]
        public void TestVariables(string mapping, int expectedVariables)
        {
            IEnumerable<string> vars = _papper.GetVariablesOf(mapping);
            Assert.Equal(expectedVariables, vars.Count());
        }

        [Theory]
        [InlineData(nameof(DB_Setup_AGV_BST1), 4034)]
        [InlineData(nameof(DB_BST1_ChargenRV), 4284)]
        public void TestWriteableVariables(string mapping, int expectedVariables)
        {
            IEnumerable<string> vars = _papper.GetWriteableVariablesOf(mapping);
            Assert.Equal(expectedVariables, vars.Count());
        }


        [Theory]
        [InlineData(nameof(DB_Setup_AGV_BST1), 51)]
        [InlineData(nameof(DB_BST3_ChargenRV), 451)]
        [InlineData(nameof(DB_BST1_ChargenRV), 452)]
        [InlineData(nameof(SampleData), 12)]
        [InlineData(nameof(SampleDataSubstruct), 2)]
        [InlineData(nameof(DB_BST_An_Abwahl_BST1), 4)]
        [InlineData(nameof(DB_BST1_Regal_1_Konfig), 56)]
        [InlineData(nameof(DB_BST1_Geraete_1_Konfig), 4)]
        [InlineData(nameof(SampleDataAccessNames), 11)]

        public void TestWriteableBlocks(string mapping, int expectedVariables)
        {
            IEnumerable<string> vars = _papper.GetVariableBlocksOf(mapping, VariableListTypes.Write);
            Assert.Equal(expectedVariables, vars.Count());
        }

        [Theory]
        [InlineData(nameof(DB_ZK_Storage_BandG), 1206)]
        [InlineData(nameof(DB_Setup_AGV_BST1), 12)]
        [InlineData(nameof(DB_BST3_ChargenRV), 1)]
        [InlineData(nameof(DB_BST1_ChargenRV), 1)]
        [InlineData(nameof(SampleData), 14)]
        [InlineData(nameof(SampleDataSubstruct), 3)]
        [InlineData(nameof(DB_BST_An_Abwahl_BST1), 1)]
        [InlineData(nameof(DB_BST1_Regal_1_Konfig), 2)]
        [InlineData(nameof(DB_BST1_Geraete_1_Konfig), 1)]
        [InlineData(nameof(SampleDataAccessNames), 13)]
        [InlineData(nameof(DI_PGS_BST1), 327)]

        public void TestReadableBlocks(string mapping, int expectedVariables)
        {
            IEnumerable<string> vars = _papper.GetVariableBlocksWithNotAccessiblyListOf(mapping, VariableListTypes.Read, out var notAccessible);
            var cc = notAccessible.ToList();
            Assert.Equal(expectedVariables, vars.Count());
        }


        [Fact]
        public void TestSerialisation3()
        {
            string mapping = "DB_Setting_BST1";
            Dictionary<string, object> accessDict = new()
            {
                    { "\"E79.4 von GeräteSS X0 FOM060 links zuweisen\"", true },
                    { "\"E79.5[]\"", true}
                };

            Test(mapping, accessDict, false);
        }

        [Fact]
        public void BitAccessTest()
        {
            string mapping = "DB_Safety";
            Dictionary<string, object> accessDict = new()
            {
                    { "SafeMotion.Slots[0].Commands.TakeoverPermitted", true },
                    { "SafeMotion.Slots[0].Commands.TakeoverRefused", true},
                    { "SafeMotion.Slots[0].Motion.ManualOperation1", true},
                    { "SafeMotion.Slots[0].Motion.ManualOperation2", true},
                    { "SafeMotion.Slots[42].Commands.TakeoverPermitted", true },
                    { "SafeMotion.Slots[42].Commands.TakeoverRefused", true},
                    { "SafeMotion.Slots[42].Motion.ManualOperation1", true},
                    { "SafeMotion.Slots[42].Motion.ManualOperation2", true},
                    { "SafeMotion.Slots[160].Commands.TakeoverPermitted", true },
                    { "SafeMotion.Slots[160].Commands.TakeoverRefused", true},
                    { "SafeMotion.Slots[160].Motion.ManualOperation1", true},
                    { "SafeMotion.Slots[160].Motion.ManualOperation2", true},
                    { "SafeMotion.Slots[254].Commands.TakeoverPermitted", true },
                    { "SafeMotion.Slots[254].Commands.TakeoverRefused", true},
                    { "SafeMotion.Slots[254].Motion.ManualOperation1", true},
                    { "SafeMotion.Slots[254].Motion.ManualOperation2", true},
                };

            Test(mapping, accessDict, false);
        }

        [Fact]
        public void Int16AccessTest()
        {
            string mapping = "DB_Safety";
            Dictionary<string, object> accessDict = new()
            {
                    { "SafeMotion.Header.NumberOfActiveSlots",(short) 1 },
                    { "SafeMotion.Slots[0].AggregateDBNummer", (short)2},
                    { "SafeMotion.Slots[0].AggregateOffset", (short)3},
                    { "SafeMotion.Slots[0].SafeSlotVersion", (short)4},
                    { "SafeMotion.Slots[100].AggregateDBNummer", (short)5},
                    { "SafeMotion.Slots[100].AggregateOffset",(short)6},
                    { "SafeMotion.Slots[100].SafeSlotVersion", (short)7},
                    { "SafeMotion.Slots[254].AggregateDBNummer", (short)8},
                    { "SafeMotion.Slots[254].AggregateOffset", (short)9},
                    { "SafeMotion.Slots[254].SafeSlotVersion", (short)10},
                };

            Test(mapping, accessDict, (short)0);
        }

        [Fact]
        public void UInt16AccessTest()
        {
            string mapping = "DB_Safety";
            Dictionary<string, object> accessDict = new()
            {
                    { "SafeMotion.Slots[0].UnitChecksum", (ushort)2},
                    { "SafeMotion.Slots[100].UnitChecksum", (ushort)5},
                    { "SafeMotion.Slots[254].UnitChecksum", (ushort)8},

                };

            Test(mapping, accessDict, (ushort)0);
        }

        [Fact]
        public void UInt32AccessTest()
        {
            string mapping = "DB_Safety";
            Dictionary<string, object> accessDict = new()
            {
                    { "SafeMotion.Slots[0].HmiId", (uint)3},
                    { "SafeMotion.Slots[0].AccessRightReqFromHmiId", (uint)4},
                    { "SafeMotion.Slots[100].HmiId",(uint)6},
                    { "SafeMotion.Slots[100].AccessRightReqFromHmiId", (uint)7},
                    { "SafeMotion.Slots[254].HmiId", (uint)9},
                    { "SafeMotion.Slots[254].AccessRightReqFromHmiId", (uint)10},
                };

            Test(mapping, accessDict, (uint)0);
        }

        [Fact]
        public void DateAccessTest()
        {
            string mapping = "DB_Safety";
            Dictionary<string, object> accessDict = new()
            {
                    { "SafeMotion.Header.Generated", Normalize(DateTime.Now)},
                    { "SafeMotion.Slots[0].UnitTimestamp", Normalize(DateTime.Now)},
                    { "SafeMotion.Slots[100].UnitTimestamp", Normalize(DateTime.Now)},
                    { "SafeMotion.Slots[254].UnitTimestamp",Normalize( DateTime.Now)},
                };

            Test(mapping, accessDict, new DateTime(1990, 1, 1));  //01.01.1900
        }


        [Fact]
        public void SingleAccessTest()
        {
            string mapping = "PrimitiveValuesMapping";
            Dictionary<string, object> accessDict = new()
            {
                    { "Single", (float)2.2},
                };

            Test(mapping, accessDict, (float)0);
        }

        [Fact]
        public void ArrayElementsAccessTest()
        {
            string mapping = "ARRAY_TEST_MAPPING_1";
            Dictionary<string, object> accessDict = new()
            {
                    { "ByteElements[10]", (byte)0x05},
                    { "ByteElements[5]", (byte)0x06},
                    { "ByteElements[1]", (byte)0x07},
                    { "CharElements[10]", 'a'},
                    { "CharElements[5]", 'b'},
                    { "CharElements[1]", 'c'},
                    { "IntElements[10]", 10},
                    { "IntElements[5]", 20},
                    { "IntElements[1]", 30},
                };


            Test(mapping, accessDict.Take(3).ToDictionary(kvp => kvp.Key, kvp => kvp.Value), default(byte));
            Test(mapping, accessDict.Skip(3).Take(3).ToDictionary(kvp => kvp.Key, kvp => kvp.Value), default(char));
            Test(mapping, accessDict.Skip(6).Take(3).ToDictionary(kvp => kvp.Key, kvp => kvp.Value), default(int));
        }

        [Fact]
        public void BigByteArrayAccessTest()
        {
            string mapping = "ARRAY_TEST_MAPPING_2";
            Dictionary<string, object> accessDict = new()
            {
                    { "BigByteArray", Enumerable.Repeat<byte>(0x01,50000).ToArray()},
                };

            Test(mapping, accessDict, Enumerable.Repeat<byte>(0x00, 50000).ToArray());
        }

        [Fact]
        public void BigCharArrayAccessTest()
        {
            string mapping = "ARRAY_TEST_MAPPING_3";
            Dictionary<string, object> accessDict = new()
            {
                    { "BigCharArray", Enumerable.Repeat<char>('a',50000).ToArray()},
                };

            Test(mapping, accessDict, Enumerable.Repeat<char>(default, 50000).ToArray());
        }

        [Fact]
        public void BigIntArrayAccessTest()
        {
            string mapping = "ARRAY_TEST_MAPPING_4";
            Dictionary<string, object> accessDict = new()
            {
                    { "BigIntArray", Enumerable.Repeat(2,5000).ToArray()},
                };

            Test(mapping, accessDict, Enumerable.Repeat(0, 5000).ToArray());
        }



        [Fact]
        public void TestStructuralAccessWithReadonlyttributes()
        {
            string mapping = "SampleData";
            SampleData header = new()
            {
                UInt16 = 1,
                Int16 = 2,
                UInt32 = 3,
                Int32 = 4,
                Single = 5,
                Char = 'c',
                Bit1 = true,
                Bit2 = true,
                Bit3 = true,
                Bit4 = true,
                Bit5 = true,
                Bit6 = true,
                Bit7 = true,
                Bit8 = true
            };

            Dictionary<string, object> accessDict = new()
            {
                    { "This", header},
                };

            //var result = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            //Assert.Equal(accessDict.Count, result.Length);
            PlcWriteResult[] writeResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();
            foreach (PlcWriteResult item in writeResults)
            {
                Assert.Equal(ExecutionResult.Ok, item.ActionResult);
            }
            PlcReadResult[] result2 = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result2.Length);
            //Assert.False(AreDataEqual(result, result2));

            //Assert.NotEqual(header.UInt16, (result2.FirstOrDefault().Value as SampleData).UInt16);
            //Assert.False((result2.FirstOrDefault().Value as SampleData).Bit1);
        }


        [Fact]
        public void TestStructuralAccess()
        {
            string mapping = "DB_Safety2";
            UDT_SafeMotionHeader origin = new()
            {
                Generated = DateTime.MinValue,
                NumberOfActiveSlots = 0,
                Commands = new UDT_SafeMotionHeader_Commands
                {
                    AllSlotsLocked = false,
                    UpdateAllowed = false
                },
                States = new UDT_SafeMotionHeader_States
                {
                    ChecksumInvalid = false,
                    UpdateRequested = false
                }
            };

            UDT_SafeMotionHeader header = new()
            {
                Generated = Normalize(DateTime.Now),
                NumberOfActiveSlots = 2,
                Commands = new UDT_SafeMotionHeader_Commands
                {
                    AllSlotsLocked = true,
                    UpdateAllowed = true
                },
                States = new UDT_SafeMotionHeader_States
                {
                    ChecksumInvalid = true,
                    UpdateRequested = true
                }
            };

            Dictionary<string, object> accessDict = new()
            {
                    { "SafeMotion.Header", header},
                };
            PlcWriteResult[] writeOriginResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, new Dictionary<string, object> {
                    { "SafeMotion.Header", origin},
                }.ToArray()).ToArray()).GetAwaiter().GetResult();


            PlcReadResult[] result = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result.Length);
            PlcWriteResult[] writeResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();
            foreach (PlcWriteResult item in writeResults)
            {
                Assert.Equal(ExecutionResult.Ok, item.ActionResult);
            }
            PlcReadResult[] result2 = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result2.Length);
            Assert.False(AreDataEqual(result, result2));


            // Assert.True(AreDataEqual(ToExpando(header), result2.Values.FirstOrDefault()));
        }


        [Fact]
        public void TestStructuralAllAccess()
        {
            string mapping = "DB_Safety2";

            Stopwatch t = new();
            t.Start();
            PlcReadResult[] result = _papper.ReadAsync(PlcReadReference.FromAddress($"{mapping}")).GetAwaiter().GetResult();
            t.Stop();
        }


        [Fact]
        public void TestBitAccess()
        {
            string mapping = "DB_TestCTT";

            Stopwatch t = new();
            t.Start();
            PlcReadResult[] result1 = _papper.ReadAsync(PlcReadReference.FromAddress($"{mapping}.StartTrack")).GetAwaiter().GetResult();
            PlcReadResult[] result2 = _papper.ReadAsync(PlcReadReference.FromAddress($"{mapping}.StartTrack"), PlcReadReference.FromAddress($"{mapping}.StartEinlauf"), PlcReadReference.FromAddress($"{mapping}.StartAuslauf")).GetAwaiter().GetResult();
            t.Stop();
        }


        [Fact]
        public async Task MixedAccessTest()
        {
            string mapping = "DB_Safety2";

            Stopwatch t = new();
            t.Start();
            Task<PlcReadResult[]> result1 = _papper.ReadAsync(PlcReadReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots"),
                                           PlcReadReference.FromAddress($"{mapping}.SafeMotion.Header.States.ChecksumInvalid"));

            PlcReadResult[] result2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress($"{mapping}.SafeMotion") }).ConfigureAwait(false);
            await result1.ConfigureAwait(false);
            t.Stop();
        }

        [Fact]
        public async Task MixedAccessWithDataChangeTest()
        {
            string mapping = "DB_Safety2";

            Stopwatch t = new();
            t.Start();
            short value = -1;
            await _papper.WriteAsync(PlcWriteReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", (short)0)).ConfigureAwait(false);

            Subscription sub = _papper.SubscribeDataChanges((s, e) =>
            {
                if (e.FieldCount > 0)
                {
                    value = (short)e[$"{mapping}.SafeMotion.Header.NumberOfActiveSlots"];
                }
            }, PlcWatchReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", 10),
                                                                  PlcWatchReference.FromAddress($"{mapping}.SafeMotion.Header.States.ChecksumInvalid", 10));
            PlcReadResult[] result2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress($"{mapping}.SafeMotion") }).ConfigureAwait(false);

            await _papper.WriteAsync(PlcWriteReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", (short)1)).ConfigureAwait(false);

            await Task.Delay(2000).ConfigureAwait(false);

            Assert.Equal((short)1, value);

            sub.Dispose();
            t.Stop();
        }

        [Fact]
        public async Task MixedAccessWithDataChangeTest2()
        {
            string mapping = "DB_Safety2";

            Stopwatch t = new();
            t.Start();
            short value = -1;
            await _papper.WriteAsync(PlcWriteReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", (short)0)).ConfigureAwait(false);

            PlcReadResult[] result2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress($"{mapping}.SafeMotion") }).ConfigureAwait(false);
            Subscription sub = _papper.SubscribeDataChanges((s, e) =>
            {
                if (e.FieldCount > 0)
                {
                    value = (short)e[$"{mapping}.SafeMotion.Header.NumberOfActiveSlots"];
                }
            }, PlcWatchReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", 10),
                                                                  PlcWatchReference.FromAddress($"{mapping}.SafeMotion.Header.States.ChecksumInvalid", 10));


            await _papper.WriteAsync(PlcWriteReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", (short)1)).ConfigureAwait(false);

            await Task.Delay(2000).ConfigureAwait(false);

            Assert.Equal((short)1, value);

            sub.Dispose();
            t.Stop();
        }



        [Fact]
        public async Task DataChanegOnBitsTest()
        {
            Stopwatch t = new();
            t.Start();
            await _papper.WriteAsync(PlcWriteReference.FromAddress($"DB_IDAT_MSpindleData1.IDATInterface.IDATtoPLC.Toggle", false)).ConfigureAwait(false);

            Subscription sub = _papper.SubscribeDataChanges((s, e) =>
            {

            }, PlcWatchReference.FromAddress($"DB_IDAT_MSpindleData.IDATInterface.PLCtoIDAT.UpdateRequired", 10),
               PlcWatchReference.FromAddress($"DB_IDAT_MSpindleData.IDATInterface.IDATtoPLC.Toggle", 10),
               PlcWatchReference.FromAddress($"DB_IDAT_MSpindleData.IDATInterface.PLCtoIDAT.WriteEnable", 10));

            await Task.Delay(100).ConfigureAwait(false);
            await _papper.WriteAsync(PlcWriteReference.FromAddress($"DB_IDAT_MSpindleData.IDATInterface.PLCtoIDAT.UpdateRequired", true)).ConfigureAwait(false);
            //await Task.Delay(100);
            //await _papper.WriteAsync(PlcWriteReference.FromAddress($"DB_IDAT_MSpindleData.IDATInterface.IDATtoPLC.Toggle", false));

            await Task.Delay(30000).ConfigureAwait(false);

            sub.Dispose();
            t.Stop();
        }

        [Fact]
        public async Task TestWriteSerializedData()
        {
            PlcDataMapperSerializer ser = new();

            byte[] value = ser.Serialize(TimeTransformationRule.FromTimeZoneInfo(TimeZoneInfo.Local));



            await _papper.WriteAsync(PlcWriteReference.FromAddress($"DB301.S108,5", CultureInfo.CurrentCulture.Name),
                                    PlcWriteReference.FromAddress($"DB301.B10,{value.Length}", value),
                                    PlcWriteReference.FromAddress($"DB301.DT2", DateTime.Now),
                                    PlcWriteReference.FromAddress($"DB301.X106.0", true)).ConfigureAwait(false);
        }


        [Fact]
        public void ArrayIndexAccessTest()
        {
            string mapping = "ARRAY_TEST_MAPPING_5";
            Dictionary<string, object> accessDict = new()
            {
                    { "BigCharArray[1]", 'X'},
                };

            Test(mapping, accessDict, default(char));
        }


        [Fact]
        public void TODTest()
        {
            string mapping = "STRING_ARRAY_TEST_MAPPING_1";
            Dictionary<string, object> accessDict = new()
            {
                    { "Time[1]", TimeSpan.FromHours(12)},
                    { "Time[2]", TimeSpan.FromHours(1)},
                    { "Time[3]", TimeSpan.FromSeconds(10)},
                    { "Time[4]", TimeSpan.FromMilliseconds(111)},
                };

            Test(mapping, accessDict, new TimeSpan(0));


        }


        [Fact]
        public void ReadNonExistingValue()
        {
            string mapping = "STRING_ARRAY_TEST_MAPPING";
            Dictionary<string, object> accessDict = new()
            {
                    { "XXX", "TEST1"}
                };

            Assert.Throws<InvalidVariableException>(() =>
           {
               PlcReadResult[] result = _papper.ReadAsync(PlcReadReference.FromRoot(mapping, accessDict.Keys.ToArray()).ToArray()).GetAwaiter().GetResult();
               Assert.Empty(result);
           });
        }

        [Fact]
        public void ConvertTest()
        {
            Span<byte> data = new(new byte[] { 0x01, 0x02, 0x03, 0x04 });

            int v2 = BinaryPrimitives.ReadInt32BigEndian(data);
            int v3 = BinaryPrimitives.ReadInt32LittleEndian(data);

            Span<byte> data1 = new(new byte[4]);
            float s = 25.4f;
            BinaryPrimitives.WriteInt32BigEndian(data1, Convert.ToInt32(s));
            float res = Convert.ToSingle(BinaryPrimitives.ReadInt32BigEndian(data1));

            Span<byte> data4 = new(new byte[4]);
            BinaryPrimitives.WriteSingleBigEndian(data4, s);
            float x4 = BinaryPrimitives.ReadSingleBigEndian(data4);
        }




        [Fact]
        public async Task TestInvalidMappings()
        {

            using PlcDataMapper papper = new(960, Papper_OnRead, Papper_OnWrite, UpdateHandler, ReadMetaData, OptimizerType.Items);
            papper.AddMapping(typeof(DB_Safety));

            using Subscription subscription = papper.CreateSubscription(ChangeDetectionStrategy.Event);
            Assert.True(await subscription.TryAddItemsAsync(PlcWatchReference.FromAddress("DB_Safety.SafeMotion.Slots[0]", 100)).ConfigureAwait(false));
            Assert.False(await subscription.TryAddItemsAsync(PlcWatchReference.FromAddress("Test.XY", 100)).ConfigureAwait(false));
            Assert.False(await subscription.TryAddItemsAsync(PlcWatchReference.FromAddress("DB_Safety.XY", 100)).ConfigureAwait(false));


            await Assert.ThrowsAsync<InvalidVariableException>(() => subscription.AddItemsAsync(PlcWatchReference.FromAddress("Test.XY", 100))).ConfigureAwait(false);
            await Assert.ThrowsAsync<InvalidVariableException>(() => subscription.AddItemsAsync(PlcWatchReference.FromAddress("DB_Safety.XY", 100))).ConfigureAwait(false);
        }

        [Fact]
        public void TestGetAddressOf()
        {
            string mapping = "DB_Safety2";
            PlcItemAddress result = _papper.GetAddressOf(PlcReadReference.FromAddress($"{mapping}.SafeMotion.Slots"));


            Assert.Equal(14, result.Offset.Bytes);
            Assert.Equal(8670, result.Size.Bytes);

        }


        [Fact]
        public void TestGetAddressOfWString()
        {
            string mapping = "DB_MotionHMI";
            PlcItemAddress result = _papper.GetAddressOf(PlcReadReference.FromAddress($"{mapping}.HMI.MotionLine[8].Txt.Position[1]"));


            Assert.Equal(9442, result.Offset.Bytes);
            Assert.Equal(54, result.Size.Bytes);

        }

        [Fact]
        public void TestGetAddressOfType()
        {
            string mapping = "DB_MotionHMI";
            PlcItemAddress result1 = _papper.GetAddressOf(PlcReadReference.FromAddress($"{mapping}.HMI.MotionLine[8].Txt.Position[1]"));

            mapping = "DB_Safety2";
            PlcItemAddress result2 = _papper.GetAddressOf(PlcReadReference.FromAddress($"{ mapping}.SafeMotion.Slots"));
            PlcItemAddress result3 = _papper.GetAddressOf(PlcReadReference.FromAddress($"{ mapping}.SafeMotion.Slots[2]"));
            PlcItemAddress result4 = _papper.GetAddressOf(PlcReadReference.FromAddress($"{ mapping}.SafeMotion.Slots[2].SlotId"));
            PlcItemAddress result5 = _papper.GetAddressOf(PlcReadReference.FromAddress($"{ mapping}.SafeMotion.Slots[2].SafeSlotVersion"));



        }


        [Fact]
        public void TestWriteRawDataToStruct()
        {
            string mapping = "DB_Safety2";
            UDT_SafeMotionHeader header = new()
            {
                Generated = Normalize(DateTime.Now),
                NumberOfActiveSlots = 2,
                Commands = new UDT_SafeMotionHeader_Commands
                {
                    AllSlotsLocked = true,
                    UpdateAllowed = true
                },
                States = new UDT_SafeMotionHeader_States
                {
                    ChecksumInvalid = true,
                    UpdateRequested = true
                }
            };

            Dictionary<string, object> accessDict = new()
            {
                    { "SafeMotion.Header", header},
                };

            PlcDataMapperSerializer s = new();
            accessDict["SafeMotion.Header"] = s.Serialize(header);

            PlcWriteResult[] writeResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();
        }



        [Fact]
        public async Task ReadBitsAsyncTest()
        {
            PlcReadResult[] x = await _papper.ReadAsync(PlcReadReference.FromAddress("DB_IDAT_MSpindleData.IDATInterface.PLCtoIDAT.WriteEnable")).ConfigureAwait(false);
        }


        [Fact]
        public async Task AddAndRemoveMapping()
        {
            bool result1 = _papper.AddMapping(typeof(UdtMotion), new Attributes.MappingAttribute
            (
                "DI_VK_BST1.Index.bew",
                "DB1111",
                100
            ));


            await _papper.ReadAsync(PlcReadReference.FromAddress("DI_VK_BST1.Index.bew.MovingState1")).ConfigureAwait(false);


        }

        [Fact]
        public void ReadIndexedItem()
        {

            List<Internal.Execution> exec1 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number[1]", 200) });
            List<Internal.Execution> exec2 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_IPSC_Konfig.StatusAnzeige_ZP[1].ErrText", 200) });
            List<Internal.Execution> exec3 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA", 200) });


        }






        [Fact]
        public async Task ReadSmallAndBigPart()
        {
            PlcWriteReference write = PlcWriteReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", new char[4] { 'A', 'B', 'C', 'D' });

            List<Internal.Execution> exec = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", 200) });
            List<Internal.Execution> exec2 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA", 200) });

            Assert.True(exec[0].Bindings.Values.FirstOrDefault().RawData.Size == 4);
            Assert.True(exec2[0].Bindings.Values.FirstOrDefault().RawData.Size == 7970);


            await _papper.WriteAsync(write).ConfigureAwait(false);


            PlcReadResult[] data2 = await _papper.ReadAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number") }).ConfigureAwait(false);
            Assert.True(data2[0].Value is char[] b2 && b2.Length == 4);

        }

        #region Helper


        private void Test<T>(string mapping, Dictionary<string, object> accessDict, T defaultValue)
        {
            //Initial read to ensure all are false
            _papper.WriteAsync(accessDict.Select(c => PlcWriteReference.FromAddress($"{mapping}.{c.Key}", defaultValue))).GetAwaiter().GetResult();
            PlcReadReference[] toRead = accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray();
            PlcReadResult[] result = _papper.ReadAsync(toRead).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result.Length);
            foreach (PlcReadResult item in result)
            {
                Assert.Equal(defaultValue, (T)item.Value);
            }

            //Write the value
            _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();

            //Second read to ensure correct written
            result = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result.Length);
            foreach (PlcReadResult item in result)
            {
                Assert.Equal((T)accessDict[item.Variable], (T)item.Value);
            }
        }

        /// <summary>
        /// This method remove the nanoseconds from the date time, because plc's could not handle that
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static DateTime Normalize(DateTime dt)
        {
            return dt.AddTicks((dt.Ticks % 10000) * -1);
        }

        private Task UpdateHandler(IEnumerable<DataPack> monitoring, bool add = true)
        {
            //foreach (var item in monitoring)
            //{
            //    _mockPlc.UpdateDataChangeItem(item, !add);
            //}
            return Task.CompletedTask;
        }

        private Task ReadMetaData(IEnumerable<MetaDataPack> packs)
        {
            foreach (MetaDataPack item in packs)
            {
                item.ExecutionResult = ExecutionResult.Error;
            }
            return Task.CompletedTask;
        }

        private static Task Papper_OnRead(IEnumerable<DataPack> reads)
        {
            List<DataPack> result = reads.ToList();
            foreach (DataPackSymbolic item in result.OfType<DataPackSymbolic>())
            {
                Console.WriteLine($"OnRead: SymbolicName:{item.SymbolicName};");
                if (_mockPlc.TryGetValue(item.SymbolicName, out object val))
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
            List<DataPack> result = reads.ToList();
            foreach (DataPackSymbolic item in result.OfType<DataPackSymbolic>())
            {
                _mockPlc[item.SymbolicName] = item.Value;
                item.ApplyResult(ExecutionResult.Ok);
            }
            return Task.CompletedTask;
        }


        private static void SetPropertyInExpandoObject(dynamic parent, string address, object value)
        {
            SetPropertyInExpandoObject(parent, address.Replace("[", ".[", StringComparison.InvariantCultureIgnoreCase).Split('.'), value);
        }

        private static void SetPropertyInExpandoObject(dynamic parent, IEnumerable<string> parts, object value)
        {
            string key = parts.First();
            parts = parts.Skip(1);
            if (parent is IList<object> list)
            {
                int index = int.Parse(key.TrimStart('[').TrimEnd(']'), CultureInfo.InvariantCulture);
                if (parts.Any())
                {
                    SetPropertyInExpandoObject(list.ElementAt(index), parts, value);
                }
                else
                {
                    list[index] = value;
                }
            }
            else
            {
                if (parent is IDictionary<string, object> dictionary)
                {
                    if (parts.Any())
                    {
                        SetPropertyInExpandoObject(dictionary[key], parts, value);
                    }
                    else
                    {
                        dictionary[key] = value;
                    }
                }
            }
        }

        private static object GetPropertyInExpandoObject(dynamic parent, string address)
        {
            return GetPropertyInExpandoObject(parent, address.Replace("[", ".[", StringComparison.InvariantCultureIgnoreCase).Split('.'));
        }

        private static object GetPropertyInExpandoObject(dynamic parent, IEnumerable<string> parts)
        {
            string key = parts.First();
            parts = parts.Skip(1);
            if (parent is IList<object> list)
            {
                int index = int.Parse(key.TrimStart('[').TrimEnd(']'), CultureInfo.InvariantCulture);
                if (parts.Any())
                {
                    return GetPropertyInExpandoObject(list.ElementAt(index), parts); // TODO
                }
                else
                {
                    return list.ElementAt(index);
                }
            }
            else
            {
                if (parent is IDictionary<string, object> dictionary)
                {
                    if (parts.Any())
                    {
                        return GetPropertyInExpandoObject(dictionary[key], parts);
                    }
                    else
                    {
                        return dictionary[key];
                    }
                }
            }
            return null;
        }


        private static ExpandoObject ToExpando<T>(T instance)
        {
            ExpandoObject obj = new();
            foreach (PropertyInfo item in instance.GetType().GetTypeInfo().DeclaredProperties.Where(x => x.GetCustomAttribute<IgnoreAttribute>()?.IsIgnored is not true))
            {
                if (!item.PropertyType.Namespace.StartsWith("System", false, CultureInfo.InvariantCulture))
                {
                    AddProperty(obj, item.Name, ToExpando(item.GetValue(instance)));
                }
                else
                {
                    AddProperty(obj, item.Name, item.GetValue(instance));
                }
            }
            return obj;
        }

        private static void AddProperty(dynamic parent, string name, object value)
        {
            List<dynamic> list = (parent as List<dynamic>);
            if (list != null)
            {
                list.Add(value);
            }
            else
            {
                if (parent is IDictionary<string, object> dictionary)
                {
                    dictionary[name] = value;
                }
            }
        }

        private static bool AreDataEqual(object obj1, object obj2)
        {
            Type t1 = obj1.GetType();
            Type t2 = obj2.GetType();

            if (t1 == t2) { return t1 != typeof(ExpandoObject) ? ElementEqual(obj1, obj2) : DynamicObjectCompare(obj1, obj2); }
            try { return ElementEqual(obj1, Convert.ChangeType(obj2, t1, CultureInfo.InvariantCulture)); } catch { }
            return false;
        }

        private static bool ElementEqual(object obj1, object obj2)
        {
            if (obj1 is IEnumerable list1 &&
                obj2 is IEnumerable list2)
            {
                IEnumerator enumerator1 = list1.GetEnumerator();
                IEnumerator enumerator2 = list2.GetEnumerator();
                while (true)
                {
                    bool e1 = enumerator1.MoveNext();
                    bool e2 = enumerator2.MoveNext();
                    if (e1 && e2)
                    {
                        if (!AreDataEqual(enumerator1.Current, enumerator2.Current))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return e1 == e2; //Length not the same?
                    }
                }
            }
            return obj1.Equals(obj2);
        }

        private static bool DynamicObjectCompare(object obj1, object obj2)
        {
            if (obj1 is IDictionary<string, object> dictionary1 &&
                obj2 is IDictionary<string, object> dictionary2)
            {
                foreach (KeyValuePair<string, object> o1 in dictionary1)
                {
                    if (!dictionary2.TryGetValue(o1.Key, out object o2) || !AreDataEqual(o1.Value, o2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Dispose()
        {
            _papper?.Dispose();
        }

        #endregion
    }
}
