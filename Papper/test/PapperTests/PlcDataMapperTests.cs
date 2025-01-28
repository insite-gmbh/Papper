using Insite.Customer.Data.___ST000_SCREW_GS_PDB;
using Insite.Customer.Data.___ST641_ZY001_MA71_PDB;
using Papper;
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
using Papper.Tests.Util;
using System;
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

//run in sequence because of db sharing
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
namespace Papper.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public sealed class PlcDataMapperTests : IDisposable
    {
        private readonly PlcDataMapper _papper = new(960, Papper_OnRead, Papper_OnWrite, OptimizerType.Items);
        private readonly ITestOutputHelper _output;

        public PlcDataMapperTests(ITestOutputHelper output)
        {
            _output = output;
            _papper.AddMapping(typeof(MultiTest_DB));
            _papper.AddMapping(typeof(___ST641_ZY001_MA71_PDB));
            _papper.AddMapping(typeof(___ST000_SCREW_GS_PDB));
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
            _papper.AddMapping(typeof(DB_Setting_BST1));
            _papper.AddMapping(typeof(DB_ZK_Storage_BandG));
            

        }

        [Theory]
        [InlineData("MultiTest_DB", 8928)]
        [InlineData(nameof(DB_Setup_AGV_BST1), 4244)]
        [InlineData(nameof(DB_Safety), 4596)]
        [InlineData("ARRAY_TEST_MAPPING_1", 105030)]
        [InlineData("STRING_ARRAY_TEST_MAPPING", 21)]
        [InlineData(nameof(PrimitiveValuesMapping), 6)]
        [InlineData(nameof(DB_MotionHMI), 725)]
        [InlineData(nameof(DB_BST1_ChargenRV), 4384)]
        [InlineData("++ST000_SCREW_GS_PDB", 8024)]
        public void TestVariables(string mapping, int expectedVariables)
        {
            var vars = _papper.GetVariablesOf(mapping);
            Assert.Equal(expectedVariables, vars.Count());
        }

        [Theory]
        [InlineData(nameof(DB_Setup_AGV_BST1), 4034)]
        [InlineData(nameof(DB_BST1_ChargenRV), 4284)]
        public void TestWriteableVariables(string mapping, int expectedVariables)
        {
            var vars = _papper.GetWriteableVariablesOf(mapping);
            Assert.Equal(expectedVariables, vars.Count());
        }



        [Theory]
        [InlineData("++ST641+ZY001-MA71_PDB", 9)]
        [InlineData("++ST000_SCREW_GS_PDB", 1)]
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
            var vars = _papper.GetVariableBlocksOf(mapping, VariableListTypes.Write);
            Assert.Equal(expectedVariables, vars.Count());
        }

        [Theory]
        [InlineData("MultiTest_DB", 1)]
        [InlineData("++ST641+ZY001-MA71_PDB", 9)]
        [InlineData("++ST000_SCREW_GS_PDB", 1)]
        [InlineData(nameof(DB_Setup_AGV_BST1), 12)]
        [InlineData(nameof(DB_BST3_ChargenRV), 1)]
        [InlineData(nameof(DB_BST1_ChargenRV), 1)]
        [InlineData(nameof(SampleData), 14)]
        [InlineData(nameof(SampleDataSubstruct), 3)]
        [InlineData(nameof(DB_BST_An_Abwahl_BST1), 1)]
        [InlineData(nameof(DB_BST1_Regal_1_Konfig), 2)]
        [InlineData(nameof(DB_BST1_Geraete_1_Konfig), 1)]
        [InlineData(nameof(SampleDataAccessNames),13)]
        [InlineData(nameof(DB_ZK_Storage_BandG),1206)]

        public void TestReadableBlocks(string mapping, int expectedVariables)
        {
            var vars = _papper.GetVariableBlocksOf(mapping, VariableListTypes.Read);
            Assert.Equal(expectedVariables, vars.Count());
        }

        [Fact]
        public void BitAccessTest()
        {
            var mapping = "DB_Safety";
            var accessDict = new Dictionary<string, object> {
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
            var mapping = "DB_Safety";
            var accessDict = new Dictionary<string, object> {
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
            var mapping = "DB_Safety";
            var accessDict = new Dictionary<string, object> {
                    { "SafeMotion.Slots[0].UnitChecksum", (ushort)2},
                    { "SafeMotion.Slots[100].UnitChecksum", (ushort)5},
                    { "SafeMotion.Slots[254].UnitChecksum", (ushort)8},

                };

            Test(mapping, accessDict, (ushort)0);
        }

        [Fact]
        public void UInt32AccessTest()
        {
            var mapping = "DB_Safety";
            var accessDict = new Dictionary<string, object> {
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
            var mapping = "DB_Safety";
            var accessDict = new Dictionary<string, object> {
                    { "SafeMotion.Header.Generated", Normalize(DateTime.Now)},
                    { "SafeMotion.Slots[0].UnitTimestamp", Normalize(DateTime.Now)},
                    { "SafeMotion.Slots[100].UnitTimestamp", Normalize(DateTime.Now)},
                    { "SafeMotion.Slots[254].UnitTimestamp",Normalize( DateTime.Now)},
                };

            Test(mapping, accessDict, new DateTime(1970, 1, 1));  //01.01.1900
        }


        [Fact]
        public void SingleAccessTest()
        {
            var mapping = "PrimitiveValuesMapping";
            var accessDict = new Dictionary<string, object> {
                    { "Single", (float)2.2},
                };

            Test(mapping, accessDict, (float)0);
        }

        [Fact]
        public void ArrayElementsAccessTest()
        {
            var mapping = "ARRAY_TEST_MAPPING_1";
            var accessDict = new Dictionary<string, object> {
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
            var mapping = "ARRAY_TEST_MAPPING_2";
            var accessDict = new Dictionary<string, object> {
                    { "BigByteArray", Enumerable.Repeat<byte>(0x01,50000).ToArray()},
                };

            Test(mapping, accessDict, Enumerable.Repeat<byte>(0x00, 50000).ToArray());
        }

        [Fact]
        public void BigCharArrayAccessTest()
        {
            var mapping = "ARRAY_TEST_MAPPING_3";
            var accessDict = new Dictionary<string, object> {
                    { "BigCharArray", Enumerable.Repeat<char>('a',50000).ToArray()},
                };

            Test(mapping, accessDict, Enumerable.Repeat<char>(default, 50000).ToArray());
        }

        [Fact]
        public void BigIntArrayAccessTest()
        {
            var mapping = "ARRAY_TEST_MAPPING_4";
            var accessDict = new Dictionary<string, object> {
                    { "BigIntArray", Enumerable.Repeat(2,5000).ToArray()},
                };

            Test(mapping, accessDict, Enumerable.Repeat(0, 5000).ToArray());
        }



        [Fact]
        public void TestStructuralAccessWithReadonlyttributes()
        {
            var mapping = "SampleData";
            var header = new SampleData
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

            var accessDict = new Dictionary<string, object> {
                    { "This", header},
                };

            //var result = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            //Assert.Equal(accessDict.Count, result.Length);
            var writeResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();
            foreach (var item in writeResults)
            {
                Assert.Equal(ExecutionResult.Ok, item.ActionResult);
            }
            var result2 = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result2.Length);
            //Assert.False(AreDataEqual(result, result2));

            //Assert.NotEqual(header.UInt16, (result2.FirstOrDefault().Value as SampleData).UInt16);
            //Assert.False((result2.FirstOrDefault().Value as SampleData).Bit1);
        }


        [Fact]
        public void TestStructuralAccess()
        {
            var mapping = "DB_Safety2";
            var header = new UDT_SafeMotionHeader
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

            var accessDict = new Dictionary<string, object> {
                    { "SafeMotion.Header", header},
                };

            var result = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result.Length);
            var writeResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();
            foreach (var item in writeResults)
            {
                Assert.Equal(ExecutionResult.Ok, item.ActionResult);
            }
            var result2 = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result2.Length);
            Assert.False(AreDataEqual(result, result2));


            // Assert.True(AreDataEqual(ToExpando(header), result2.Values.FirstOrDefault()));
        }


        [Fact]
        public void TestStructuralAccessOfRawData()
        {
            var mapping = "DB_Safety2";
            var header = new UDT_SafeMotionHeader
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

            var accessDict = new Dictionary<string, object> {
                    { "SafeMotion.Header", header},
                };

            var result = _papper.ReadBytesAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result.Length);
            var writeResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();
            foreach (var item in writeResults)
            {
                Assert.Equal(ExecutionResult.Ok, item.ActionResult);
            }
            var result2 = _papper.ReadBytesAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result2.Length);
            Assert.False(AreDataEqual(result, result2));

        }


        [Fact]
        public void TestStructuralAllAccess()
        {
            var mapping = "DB_Safety2";

            var t = new Stopwatch();
            t.Start();
            var result = _papper.ReadAsync(PlcReadReference.FromAddress($"{mapping}")).GetAwaiter().GetResult();
            Assert.NotNull(result);
            t.Stop();
        }


        [Fact]
        public async Task MixedAccessTest()
        {
            var mapping = "DB_Safety2";

            var t = new Stopwatch();
            t.Start();
            var result1 = _papper.ReadAsync(PlcReadReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots"),
                                           PlcReadReference.FromAddress($"{mapping}.SafeMotion.Header.States.ChecksumInvalid"));

            var result2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress($"{mapping}.SafeMotion") }).ConfigureAwait(false);
            Assert.NotNull(result2);
            await result1.ConfigureAwait(false);
            t.Stop();
        }

        [Fact]
        public async Task MixedAccessWithDataChangeTest()
        {
            var mapping = "DB_Safety2";

            var t = new Stopwatch();
            t.Start();
            short value = -1;
            await _papper.WriteAsync(PlcWriteReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", (short)0)).ConfigureAwait(false);

            var sub = _papper.SubscribeDataChanges((s, e) =>
            {
                if (e.FieldCount > 0)
                {
                    value = (short)e[$"{mapping}.SafeMotion.Header.NumberOfActiveSlots"];
                }
            }, PlcWatchReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", 10),
                                                                  PlcWatchReference.FromAddress($"{mapping}.SafeMotion.Header.States.ChecksumInvalid", 10));
            var result2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress($"{mapping}.SafeMotion") }).ConfigureAwait(false);

            await _papper.WriteAsync(PlcWriteReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", (short)1)).ConfigureAwait(false);

            await Task.Delay(2000).ConfigureAwait(false);

            Assert.Equal((short)1, value);

            sub.Dispose();
            t.Stop();
        }

        [Fact]
        public async Task MixedAccessWithDataChangeTest2()
        {
            var mapping = "DB_Safety2";

            var t = new Stopwatch();
            t.Start();
            short value = -1;
            await _papper.WriteAsync(PlcWriteReference.FromAddress($"{mapping}.SafeMotion.Header.NumberOfActiveSlots", (short)0)).ConfigureAwait(false);

            var result2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress($"{mapping}.SafeMotion") }).ConfigureAwait(false);
            var sub = _papper.SubscribeDataChanges((s, e) =>
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
            var t = new Stopwatch();
            t.Start();
            await _papper.WriteAsync(PlcWriteReference.FromAddress($"DB_IDAT_MSpindleData1.IDATInterface.IDATtoPLC.Toggle", false)).ConfigureAwait(false);

            var sub = _papper.SubscribeDataChanges((s, e) =>
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
        public void TestStructuralAllWithSerializerAccess()
        {
            var mapping = "DB_Safety2";

            var t = new Stopwatch();
            var ser = new PlcDataMapperSerializer();
            t.Start();
            var address = _papper.GetAddressOf(PlcReadReference.FromAddress($"{mapping}")).RawAddress<byte>();
            var result = _papper.ReadAsync(PlcReadReference.FromAddress(address)).GetAwaiter().GetResult().FirstOrDefault();
            if (result.Value is byte[] by)
            {
                var x = ser.Deserialize<DB_Safety>(by);
                Assert.NotNull(x);
            }
            else
            {
                Assert.True(false, "Invalid reslt type!");
            }
            t.Stop();
        }

        [Fact]
        public async Task TestWriteSerializedData()
        {
            var ser = new PlcDataMapperSerializer();

            var value = ser.Serialize(TimeTransformationRule.FromTimeZoneInfo(TimeZoneInfo.Local));



            await _papper.WriteAsync(PlcWriteReference.FromAddress($"DB301.S108,5", CultureInfo.CurrentCulture.Name),
                                    PlcWriteReference.FromAddress($"DB301.B10,{value.Length}", value),
                                    PlcWriteReference.FromAddress($"DB301.DT2", DateTime.Now),
                                    PlcWriteReference.FromAddress($"DB301.X106.0", true)).ConfigureAwait(false);
        }





        [Theory]
        [InlineData("DB2000.W2", (ushort)3)]
        [InlineData("DB2001.X0.0", true)]
        [InlineData("DB2002.X0_1", true)]
        [InlineData("DB2003.X0.0,8", new bool[] { false, false, true, true, false, false, true, true })]
        [InlineData("DB2004.X0.1,4", new bool[] { false, false, true, true })]
        [InlineData("DB2005.X1.1,4", new bool[] { false, false, true, true })]
        [InlineData("DB2006.X0.0,10", new bool[] { false, false, true, true, false, false, true, true, false, false })]
        [InlineData("DB2007.X0.0,16", new bool[] { false, false, true, true, false, false, true, true, false, false, true, true, false, false, true, true })]
        [InlineData("DB2008.X0.4,16", new bool[] { false, false, true, true, false, false, true, true, false, false, true, true, false, false, true, true })]
        public void PerformReadWriteRaw(string address, object value)
        {
            using var papper = new PlcDataMapper(960, Papper_OnRead, Papper_OnWrite);
            var readResults = papper.ReadAsync(PlcReadReference.FromAddress(address)).GetAwaiter().GetResult();
            var writeResults = papper.WriteAsync(PlcWriteReference.FromAddress(address, value)).GetAwaiter().GetResult();
            var afterWriteReadResults = papper.ReadAsync(PlcReadReference.FromAddress(address)).GetAwaiter().GetResult();

            Assert.Equal(value, afterWriteReadResults[0].Value);
        }



        [Theory]
        [InlineData("DB_BST1_ChargenRV.This", "Dat.Data.Element[5].MaxCntPiecesInTray", (short)5, "Dat.Data.Element[5].ActCntPieces", (short)2)]
        [InlineData("DB_BST4_Boxen_1_Konfig.This", "Boxen.vorhanden", true, "Boxen.fertig", true)]
        [InlineData("DB_BST1_Regal_1_Konfig.This", "Regal.Fach[1].aktiv", true, "Regal.Fach[1].fertig", true)]
        [InlineData("DB_IPSC_Konfig.This", "ZP[2].UNIV_aktiv", true, "ZP[2].Ausw.UNIV_Ergebnis.IO_Nr", 1)]
        [InlineData("DB_SpindlePos_BST1.This", "N57_Pos[2].PosX", 2, "ActPosX", 1)]
        
        public async Task PerformReadStruct(string address, string propertyWritable, object valueWritable, string propertyReadonly, object valueReadonly)
        {
            var readResults = await _papper.ReadAsync(PlcReadReference.FromAddress(address)).ConfigureAwait(false);
            Assert.NotNull(propertyWritable);
            Assert.NotNull(propertyReadonly);
            var res1 = GetPropertyInExpandoObject(readResults[0].Value, propertyWritable!);
            var res2 = GetPropertyInExpandoObject(readResults[0].Value, propertyReadonly!);

            SetPropertyInExpandoObject(readResults[0].Value, propertyWritable, valueWritable);
            SetPropertyInExpandoObject(readResults[0].Value, propertyReadonly, valueReadonly);

            await _papper.WriteAsync(PlcWriteReference.FromAddress(address, readResults[0].Value)).ConfigureAwait(false);


            readResults = await _papper.ReadAsync(PlcReadReference.FromAddress(address)).ConfigureAwait(false);


            var res3 = GetPropertyInExpandoObject(readResults[0].Value, propertyWritable);
            var res4 = GetPropertyInExpandoObject(readResults[0].Value, propertyReadonly);

            Assert.NotEqual(res1, res3);
            Assert.Equal(res2, res4);
        }

        [Theory]
        [InlineData("DB_Setting_BST1.\"E79.4 von GeräteSS X0 FOM060 links zuweisen\"")]
        public async Task PerformReadVariable(string address)
        {
            var readResults = await _papper.ReadAsync(PlcReadReference.FromAddress(address)).ConfigureAwait(false);
            Assert.Single(readResults);
            Assert.Equal(ExecutionResult.Ok, readResults[0].ActionResult);
        }

        [Theory]
        [InlineData("DB_BST1_ChargenRV.This")]
        [InlineData("DB_BST4_Boxen_1_Konfig.This")]
        [InlineData("DB_BST1_Regal_1_Konfig.This")]
        [InlineData("DB_IPSC_Konfig.This")]
        [InlineData("DB_SpindlePos_BST1.This")]
        [InlineData("DB_Setup_AGV_BST1.This")]
        public async Task TestWritingAlwaysTheSame(string address)
        {
            var readResultsBefore = await _papper.ReadAsync(PlcReadReference.FromAddress(address)).ConfigureAwait(false);
            for (int i = 0; i < 100; i++)
            {
                await _papper.WriteAsync(PlcWriteReference.FromAddress(address, readResultsBefore[0].Value)).ConfigureAwait(false);
                var readResultsAfter = await _papper.ReadAsync(PlcReadReference.FromAddress(address)).ConfigureAwait(false);
                Assert.True(AreDataEqual(readResultsBefore[0].Value, readResultsAfter[0].Value));
            }
        }



        [Fact]
        public void ArrayIndexAccessTest()
        {
            var mapping = "ARRAY_TEST_MAPPING_5";
            var accessDict = new Dictionary<string, object> {
                    { "BigCharArray[1]", 'X'},
                };

            Test(mapping, accessDict, default(char));
        }

        [Fact]
        public void ArrayStringAccessTest()
        {
            var mapping = "STRING_ARRAY_TEST_MAPPING";
            var accessDict = new Dictionary<string, object> {
                    { "TEXT[1]", "TEST1"},
                    { "TEXT[5]", "TEST5"},
                };

            Test(mapping, accessDict, "");

            //Byte data check
            var dbData = MockPlc.Instance.GetPlcEntry("DB30").Data;
            Assert.True(dbData[..2].Span.SequenceEqual(new byte[] { 35, 5 }));
            Assert.True(dbData.Slice(2, 5).Span.SequenceEqual("TEST1".ToByteArray(5)));

            Assert.True(dbData.Slice(152, 2).Span.SequenceEqual(new byte[] { 35, 5 }));
            Assert.True(dbData.Slice(154, 5).Span.SequenceEqual("TEST5".ToByteArray(5)));
        }


        [Fact]
        public void TODTest()
        {
            var mapping = "STRING_ARRAY_TEST_MAPPING_1";
            var accessDict = new Dictionary<string, object> {
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
            var mapping = "STRING_ARRAY_TEST_MAPPING";
            var accessDict = new Dictionary<string, object> {
                    { "XXX", "TEST1"}
                };

            Assert.Throws<InvalidVariableException>(() =>
           {
               var result = _papper.ReadAsync(PlcReadReference.FromRoot(mapping, accessDict.Keys.ToArray()).ToArray()).GetAwaiter().GetResult();
               Assert.Empty(result);
           });
        }

        [Fact]
        public async Task TestInvalidMappings()
        {

            using var papper = new PlcDataMapper(960, Papper_OnRead, Papper_OnWrite, UpdateHandler, ReadMetaData, OptimizerType.Items);
            papper.AddMapping(typeof(DB_Safety));

            using var subscription = papper.CreateSubscription(ChangeDetectionStrategy.Event);
            Assert.True(await subscription.TryAddItemsAsync(PlcWatchReference.FromAddress("DB_Safety.SafeMotion.Slots[0]", 100)).ConfigureAwait(false));
            Assert.False(await subscription.TryAddItemsAsync(PlcWatchReference.FromAddress("Test.XY", 100)).ConfigureAwait(false));
            Assert.False(await subscription.TryAddItemsAsync(PlcWatchReference.FromAddress("DB_Safety.XY", 100)).ConfigureAwait(false));


            await Assert.ThrowsAsync<InvalidVariableException>(() => subscription.AddItemsAsync(PlcWatchReference.FromAddress("Test.XY", 100))).ConfigureAwait(false);
            await Assert.ThrowsAsync<InvalidVariableException>(() => subscription.AddItemsAsync(PlcWatchReference.FromAddress("DB_Safety.XY", 100))).ConfigureAwait(false);
        }


        [Theory]
        [InlineData($"DB_MotionHMI.HMI.MotionLine[8].Txt.Position[1]", 9442, 54)]
        [InlineData($"DB_Safety2.SafeMotion.Slots", 14, 8670)]
        [InlineData($"DB_Safety2.SafeMotion.Slots[2]", 82, 34)]
        [InlineData($"DB_Safety2.SafeMotion.Slots[2].SlotId", 84, 1)]
        [InlineData($"DB_Safety2.SafeMotion.Slots[2].SafeSlotVersion", 82, 2)]
        public void TestGetAddressOfType(string address, int offset, int length)
        {
            var result = _papper.GetAddressOf(PlcReadReference.FromAddress(address));
            Assert.Equal(offset, result.Offset.Bytes);
            Assert.Equal(length, result.Size.Bytes);
        }


        [Fact]
        public void TestWriteRawDataToStruct()
        {
            var mapping = "DB_Safety2";
            var header = new UDT_SafeMotionHeader
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

            var accessDict = new Dictionary<string, object> {
                    { "SafeMotion.Header", header},
                };

            var s = new PlcDataMapperSerializer();
            accessDict["SafeMotion.Header"] = s.Serialize(header);

            var writeResults = _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();
        }



        [Fact]
        public async Task ReadBitsAsyncTest()
        {
            var x = await _papper.ReadAsync(PlcReadReference.FromAddress("DB_IDAT_MSpindleData.IDATInterface.PLCtoIDAT.WriteEnable")).ConfigureAwait(false);
        }


        [Fact]
        public async Task AddAndRemoveMapping()
        {
            var result1 = _papper.AddMapping(typeof(UdtMotion), new Attributes.MappingAttribute
            (
                "DI_VK_BST1.Index.bew",
                "DB1111",
                100
            ));


            await _papper.ReadAsync(PlcReadReference.FromAddress("DI_VK_BST1.Index.bew.MovingState1")).ConfigureAwait(false);


        }


        [Fact]
        public async Task ReadBigAndSmallPart()
        {
            var write = PlcWriteReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", new char[4] { 'A', 'B', 'C', 'D' });

            var exec2 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA", 200) });
            var exec = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", 200) });

            Assert.True(exec[0].Bindings.Values.FirstOrDefault()?.RawData.Size == 4);
            Assert.True(exec2[0].Bindings.Values.FirstOrDefault()?.RawData.Size == 7970);

            await _papper.WriteAsync(write).ConfigureAwait(false);


            var data = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA") }).ConfigureAwait(false);
            Assert.True(data[0].Value is byte[] b && b.Length == 7970);


            var data2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number") }).ConfigureAwait(false);
            Assert.True(data2[0].Value is byte[] b2 && b2.Length == 4);


            Assert.True(data2[0].Value is byte[] bX1 && data[0].Value is byte[] bx2 && bX1.SequenceEqual(bx2.SubArray(0, 4)));
        }

        [Fact]
        public async Task ReadSmallAndBigPart()
        {
            var write = PlcWriteReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", new char[4] { 'A', 'B', 'C', 'D' });

            var exec = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", 200) });
            var exec2 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA", 200) });

            Assert.True(exec[0].Bindings.Values.FirstOrDefault()?.RawData.Size == 4);
            Assert.True(exec2[0].Bindings.Values.FirstOrDefault()?.RawData.Size == 7970);


            await _papper.WriteAsync(write).ConfigureAwait(false);


            var data2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number") }).ConfigureAwait(false);
            Assert.True(data2[0].Value is byte[] b2 && b2.Length == 4);

            var data = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA") }).ConfigureAwait(false);
            Assert.True(data[0].Value is byte[] b && b.Length == 7970);



            Assert.True(data2[0].Value is byte[] bX1 && data[0].Value is byte[] bx2 && bX1.SequenceEqual(bx2.SubArray(0, 4)));
        }


        [Fact]
        public async Task ReadBigAndSmallPartsWithGap()
        {
            var write = PlcWriteReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", new char[4] { 'A', 'B', 'C', 'D' });

            var exec2 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA", 200) });
            var exec = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", 200), PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.EngineData.EngineNo", 200) });

            //Assert.True(exec[0].Bindings.Values.FirstOrDefault().RawData.Size == 4);
            //Assert.True(exec2[0].Bindings.Values.FirstOrDefault().RawData.Size == 7970);

            await _papper.WriteAsync(write).ConfigureAwait(false);


            var data = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA") }).ConfigureAwait(false);
            Assert.True(data[0].Value is byte[] b && b.Length == 7970);


            var data2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number") }).ConfigureAwait(false);
            Assert.True(data2[0].Value is byte[] b2 && b2.Length == 4);


            Assert.True(data2[0].Value is byte[] bX1 && data[0].Value is byte[] bx2 && bX1.SequenceEqual(bx2.SubArray(0, 4)));
        }

        [Fact]
        public async Task ReadBigAndSmallPartsWithoutGap()
        {
            var write = PlcWriteReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", new char[4] { 'A', 'B', 'C', 'D' });

            var exec2 = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA", 200) });
            var exec = _papper.Engine.DetermineExecutions(new List<PlcWatchReference> { PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number", 200), PlcWatchReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Status", 200) });

            Assert.True(exec[0].Bindings.Values.FirstOrDefault().RawData.Size == 5);
            Assert.True(exec2[0].Bindings.Values.FirstOrDefault().RawData.Size == 7970);

            await _papper.WriteAsync(write).ConfigureAwait(false);


            var data = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA") }).ConfigureAwait(false);
            Assert.True(data[0].Value is byte[] b && b.Length == 7970);


            var data2 = await _papper.ReadBytesAsync(new List<PlcReadReference> { PlcReadReference.FromAddress("DB_DATA_RF_BST1_PST.DATA.Course.WPC_Number") }).ConfigureAwait(false);
            Assert.True(data2[0].Value is byte[] b2 && b2.Length == 4);


            Assert.True(data2[0].Value is byte[] bX1 && data[0].Value is byte[] bx2 && bX1.SequenceEqual(bx2.SubArray(0, 4)));
        }


        #region Helper


        private void Test<T>(string mapping, Dictionary<string, object> accessDict, T defaultValue)
        {
            //Initial read to ensure all are false
            var toRead = accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray();
            var result = _papper.ReadAsync(toRead).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result.Length);
            foreach (var item in result)
            {
                Assert.Equal(defaultValue, (T?)item.Value);
            }

            //Write the value
            _papper.WriteAsync(PlcWriteReference.FromRoot(mapping, accessDict.ToArray()).ToArray()).GetAwaiter().GetResult();

            //Second read to ensure correct written
            result = _papper.ReadAsync(accessDict.Keys.Select(variable => PlcReadReference.FromAddress($"{mapping}.{variable}")).ToArray()).GetAwaiter().GetResult();
            Assert.Equal(accessDict.Count, result.Length);
            foreach (var item in result)
            {
                Assert.Equal((T?)accessDict[item.Variable], (T?)item.Value);
            }
        }

        /// <summary>
        /// This method remove the nanoseconds from the date time, because plc's could not handle that
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DateTime Normalize(DateTime dt) => dt.AddTicks((dt.Ticks % 10000) * -1);

        private Task UpdateHandler(IEnumerable<DataPack> monitoring, bool add = true)
        {
            foreach (var item in monitoring)
            {
                MockPlc.Instance.UpdateDataChangeItem(item, !add);
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

        private static Task Papper_OnRead(IEnumerable<DataPack> reads)
        {
            var result = reads.ToList();
            foreach (var item in result.OfType<DataPackAbsolute>())
            {
                Console.WriteLine($"OnRead: selector:{item.Selector}; offset:{item.Offset}; length:{item.Length}");
                var res = MockPlc.Instance.GetPlcEntry(item.Selector, item.Offset + item.Length).Data.Slice(item.Offset, item.Length);
                if (!res.IsEmpty)
                {
                    item.ApplyResult(ExecutionResult.Ok, res);
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
                var entry = MockPlc.Instance.GetPlcEntry(item.Selector, item.Offset + item.Length);
                if (!item.HasBitMask)
                {
                    Console.WriteLine($"OnWrite: selector:{item.Selector}; offset:{item.Offset}; length:{item.Length}");
                    item.Data[..item.Length].CopyTo(entry.Data.Slice(item.Offset, item.Length));
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


        private static void SetPropertyInExpandoObject(dynamic parent, string address, object value) => SetPropertyInExpandoObject(parent, address.Replace("[", ".[", StringComparison.InvariantCultureIgnoreCase).Split('.'), value);

        private static void SetPropertyInExpandoObject(dynamic parent, IEnumerable<string> parts, object value)
        {
            var key = parts.First();
            parts = parts.Skip(1);
            if (parent is IList<object> list)
            {
                var index = int.Parse(key.TrimStart('[').TrimEnd(']'), CultureInfo.InvariantCulture);
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

        private static object GetPropertyInExpandoObject(dynamic parent, string address) => GetPropertyInExpandoObject(parent, address.Replace("[", ".[", StringComparison.InvariantCultureIgnoreCase).Split('.'));

        private static object? GetPropertyInExpandoObject(dynamic parent, IEnumerable<string> parts)
        {
            var key = parts.First();
            parts = parts.Skip(1);
            if (parent is IList<object> list)
            {
                var index = int.Parse(key.TrimStart('[').TrimEnd(']'), CultureInfo.InvariantCulture);
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
            var obj = new ExpandoObject();
            if (instance != null)
            {
                foreach (var item in instance.GetType().GetTypeInfo().DeclaredProperties.Where(x => x.GetCustomAttribute<IgnoreAttribute>()?.IsIgnored is not true))
                {
                    if (item.PropertyType.Namespace?.StartsWith("System", false, CultureInfo.InvariantCulture) == true)
                    {
                        AddProperty(obj, item.Name, item!.GetValue(instance));
                    }
                    else
                    {
                        AddProperty(obj, item.Name, ToExpando(item.GetValue(instance)));
                    }
                }
            }
            return obj;
        }

        private static void AddProperty(dynamic parent, string name, object? value)
        {
            var list = (parent as List<dynamic?>);
            if (list != null)
            {
                list.Add(value);
            }
            else
            {
                if (parent is IDictionary<string, object?> dictionary)
                {
                    dictionary[name] = value;
                }
            }
        }

        private static bool AreDataEqual(object obj1, object obj2)
        {
            var t1 = obj1.GetType();
            var t2 = obj2.GetType();

            if (t1 == t2) { return t1 != typeof(ExpandoObject) ? ElementEqual(obj1, obj2) : DynamicObjectCompare(obj1, obj2); }
            try { return ElementEqual(obj1, Convert.ChangeType(obj2, t1, CultureInfo.InvariantCulture)); } catch { }
            return false;
        }

        private static bool ElementEqual(object obj1, object obj2)
        {
            if (obj1 is IEnumerable list1 &&
                obj2 is IEnumerable list2)
            {
                var enumerator1 = list1.GetEnumerator();
                var enumerator2 = list2.GetEnumerator();
                while (true)
                {
                    var e1 = enumerator1.MoveNext();
                    var e2 = enumerator2.MoveNext();
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
                foreach (var o1 in dictionary1)
                {
                    if (!dictionary2.TryGetValue(o1.Key, out var o2) || !AreDataEqual(o1.Value, o2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Dispose() => _papper?.Dispose();

        #endregion
    }
}
