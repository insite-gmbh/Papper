[![insite MyGet Build Status](https://www.myget.org/BuildSource/Badge/insite?identifier=1311b225-6a23-4112-834f-931a82266bff)](https://www.myget.org/)

# Papper
Plc Data Mapper is a library to map C# classes to plc blocks to get symbolic access to the plc.

<!-- TOC -->

- [Papper](#Papper)
- [NuGet](#nuget)
- [Description](#description)
- [Sample-Code](#sample-code)
- [Release Notes](#release-notes)

<!-- /TOC -->

# NuGet

    PM>  Install-Package Papper

# Description

Papper could be used with any S7 library, because it's a top level component. It convert's the given command to read or write commands for the S7 library.


# Sample-Code


To access the data you fist have to declare a class with a mapping attribute like the following one:
```cs

using Papper.Attributes;
using System;

namespace UnitTestSuit.Mappings
{
    public class UDT_SafeMotionHeader_States
    {
        public bool ChecksumInvalid { get; set; }
        public bool UpdateRequested { get; set; }
    }

    public class UDT_SafeMotionHeader_Commands
    {
        public bool UpdateAllowed { get; set; }
        public bool AllSlotsLocked { get; set; }
    }

    public class UDT_SafeMotionSlot_Commands
    {
        public bool TakeoverPermitted { get; set; }
        public bool TakeoverRefused { get; set; }
    }

    public class UDT_SafeMotionSlot_Handshake
    {
        public bool MotionSelected { get; set; }
        public bool Button1Pressed { get; set; }
        public bool Button2Pressed { get; set; }
        public Int16 HandshakeTime { get; set; }
    }

    public class UDT_SafeMotionSlot_Motion
    {
        public bool ManualEnable1 { get; set; }
        public bool ManualEnable2 { get; set; }
        public bool ManualOperation1 { get; set; }
        public bool ManualOperation2 { get; set; }
    }

    public class UDT_SafeMotionHeader
    {
        public DateTime Generated { get; set; }
        public Int16 NumberOfActiveSlots { get; set; }
        public UDT_SafeMotionHeader_States States { get; set; }
        public UDT_SafeMotionHeader_Commands Commands { get; set; }

    }

    public class UDT_SafeMotionSlot
    {
        public Int16 SafeSlotVersion { get; set; }
        public byte SlotId { get; set; }
        public DateTime UnitTimestamp { get; set; }
        public UInt16 UnitChecksum { get; set; }
        public Int16 AggregateDBNummer { get; set; }
        public Int16 AggregateOffset { get; set; }
        public UInt32 HmiId { get; set; }
        public UInt32 AccessRightReqFromHmiId { get; set; }
        public UDT_SafeMotionSlot_Commands Commands { get; set; }
        public UDT_SafeMotionSlot_Handshake Handshake { get; set; }
        public UDT_SafeMotionSlot_Motion Motion { get; set; }

    }

    public class UDT_SafeMotion
    {
        public UDT_SafeMotionHeader Header { get; set; }

        [ArrayBounds(0, 254)]
        public UDT_SafeMotionSlot[] Slots { get; set; }

    }

    [Mapping("DB_Safety", "DB15", 0)]
    public class DB_Safety
    {
        public UDT_SafeMotion SafeMotion { get; set; }

    }


}

```


The following code snippet is a small sample for the usage of papper:
```cs
var _papper = new PlcDataMapper(960);

_papper.OnRead += Papper_OnRead;
_papper.OnWrite += Papper_OnWrite;

_papper.AddMapping(typeof(DB_Safety));

var reference = PlcReadReference.FromAddress("DB_Safety.SafeMotion.Slots[100].UnitChecksum");

PlcReadResult[] result = await _papper.ReadAsync(reference);
_papper.WriteAsync(PlcWriteReference.FromPlcReference(reference, 100));


result = await _papper.ReadAsync(PlcReadReference.FromAddress("DB_Safety.SafeMotion.Slots[100].SlotId"));
_papper.WriteAsync(PlcWriteReference.FromAddress(result[0].Address, 200));

```


The interface to the used S7 library have to handle the following calls
```cs
private static byte[] Papper_OnRead(string selector, int offset, int length)
{
    //call s7 library to read data
}

private static bool Papper_OnWrite(string selector, int offset, byte[] data, byte bitMask = 0)
{
    //call s7 library to write data
}
```




# Absolute addressing
Syntax:


```[Selector].[TYPE][OFFSET],[CountOfData]```

Bool

```[Selector].[TYPE][OFFSET_Byte].[OFFSET_Bit],[CountOfData]```

String

```[Selector].[TYPE][OFFSET_Stringlength][_CountOfData]```


## Selector

* IB: Input Area
* FB: Flag Area
* QB: Output Area
* TM: Timer Area
* CT: Counter Area
* DB[Number]: DataBlock Area

## Type

* Boolean: 	X or BIT
* Byte:    	B or BYTE
* Char:    	C or CHAR
* Date:    	DATE
* DateTime:	DT or DATETIME
* DInt:    	DI or DINT
* DWord:   	DW or DWORD
* Int:     	I or INT
* LDateTime:LDT or LDATETIME
* LInt:		LI or LINT
* Time:		LT or LTIME
* LWord:	LW or LWORD
* Real:		R or REAL
* S5Time:	TIMEBCD
* Counter:  CT or COUNT
* SInt:		SI or SINT
* String:   S or STRING
* Time:     T or TIme
* TimeOfDay:TOD
* UDInt:	UDI or UDINT
* UInt: 	UI or UINT
* ULInt:	ULI or ULINT
* USInt:	USI or USINT
* WChar:	WC or WCHAR
* Word:     W or WORD
* WString:  WS or WSTRING 



# Attributes

Papper uses a couple of attributes to specify the mapping of the plc data to a dotnet class.

## Mapping

```cs
[Mapping("DB_Safety", "DB15", 0, 0)]
```
```cs
MappingAttribute(string name, string selector, int offset = 0, int observationRate = 0)
```

* name: The name of the block, this will used from papper to find the class by this name.
* selector: This parameter specifies the selector for the access library. (in case of dacs7 this is the absolute db name, in case of opcua, this is the symbolic name)
* offset: specify an offset in the data block, if you do not define the full block.  
* observationRate: this can be used by the access lib to modify the change detection for this block.  


## MappingOffset

In the case you have some parts in your block you do not like to specify, you can skip this part.

```cs
class MySymbolic
{
    bool I0_0 {get; set;}  // EA access this is I0.0

    [MappingOffset(200)]
    bool I200_0 {get; set;}  // EA access this is I200.0

    [MappingOffset(200, 3)]
    bool I400_3 {get; set;}  // EA access this is I400.3
}

```


```cs
MappingOffsetAttribute(int byteOffset, int bitOffset = -1)
```

## ArrayBounds

Because the plc has no dynamic array length, you have to specify the dimensions and the length in the attribute.
And additionally .net start by 0 but the plc can start anywhere.


```cs
[ArrayBounds(1, 10, 0)]
public bool[] NotFull { get; set; } 
```

```cs
ArrayBoundsAttribute(int from, int to, int dimension = 0)
```
## StringLength

Define the length of the string.

```cs
[PlcType("WString")]
[StringLength(70)]
public string Caption { get; set; }

[PlcType("WString")]
[ArrayBounds(0, 15, 0)]
[StringLength(25)]
public string[] Position { get; set; }
```

## PlcType

To use the correct plc type especially in the case of DateTime and Time types, you are able to specify this attribute:

It can be used on a class or and a property. By using it on a property, you target the following:

### Attribute on Property

The mapping of a .net datatype to a plc datatype is normally be done by an internal mapping list, the following table shows the default mappings:

| .net         | Plc TypeName        |
|--------------|---------------------|
| bool         | bit, bool           | 
| byte         | byte                | 
| sbyte        | sint | 
| short        | int | 
| int          | dint | 
| double       | lreal | 
| long         | lint | 
| ushort       | word | 
| uint         | dword | 
| ulong        | lword | 
| DateTime     | dateandtime | 
| TimeSpan     | time | 
| string       | string | 
| float        | real | 
| char         | char | 

If you need a more complex type mapping, you are able to add the PlcTypeAttribute on your properties, or use the Serializer method with the Type name in case you need to convert a single value.

| Plc TypeName | .net         | size in bytes |
|--------------|---------------------|----------|
|S5Time | TimeSpan | 2 |
|TimeOfDay | TimeSpan | 4 |
|LTimeOfDay |TimeSpan | 8 |
|Bit | bool | 0.1 (single bit) |
|Bool | bool | 0.1 (single bit) |
|Byte | byte | 1 |
|SInt | sbyte | 1 |
|USInt | byte | 1 |
|Int | short | 2 |
|DInt | int  | 4 |
|UDInt | uint | 4 |
|LInt | long | 8 |
|ULInt | ulong | 8 |
|Word | ushort | 2 |
|DWord | uint | 4 |
|LWord | ulong | 8 |
|DateTime | DateTime | 8 |
|Date  | DateTime | 2 |
|LDateTime | DateTime | 8 |
|DateTimeL | DateTime | 12 |
|LDT | DateTime | 8 |
|DTL | DateTime | 12 |
|Time | TimeSpan | 4 |
|LTime | TimeSpan | 8 |
|String | string | 2 + strLength |
|WString | string | 4 + (strLength * 2) |
|Real | float | 4 |
|LReal | double | 8 |
|Float |  float | 4 |
|Char | char | 1 |
|WChar | string | 2
|S7Counter | int | 2 |


### Attribute on Class

In some cases, you need to know the plc type of a class (e.g. the udt name).
In this case you are able to add this attribute to your class, to use the correct name.
This can be used if you have a special character in your udt name, or also in case of opcua where you need the udtname + the name of a substructure.


## ReadOnly

This attribute is self describing I think.

## NotAccessible

This property is not accessible, but we had defined it for the automatic offset calculation.


## SymbolicAccessName

You can generate a data structure from code, and if the variables names not match with the access name, you can redefine the names by adding a SymbolicAccessName on the property.

## AliasName

You can generate a data structure from code, and if the variables names not match with the names in your application, you can redefine the names by adding a AliasName on the property.

## MetaTag

Transport additional information which can be used by an application.

```cs
MetaTagAttribute(string name, object value)
```


# PlcDataMapperSerializer

Additionally to access library papper also provides a data serializer. With this you are able to read your data as bytes and deserialize it to a .net class. Or you can build your .net class and serialize it to a byte array which can then be downloaded to the plc.


## Sample Class
```cs


    public class TimeTransformationRule
    {
        public short Bias { get; set; } = 0; // Time bias of standard local time to UTC [min]
        public short DaylightBias { get; set; } = 0; //Time bias of local daylight saving time to local standard time [min]
        public byte DaylightStartMonth { get; set; } = 0; //Month of change to daylight saving time
        public byte DaylightStartWeek { get; set; } = 0;//Week of change to daylight saving time: 1= 1st occurrence of the weekday in the month, ..., 5= last occurrence
        public byte DaylightStartWeekday { get; set; } = 0; //Weekday of change to daylight saving time: 1= Sunday
        public byte DaylightStartHour { get; set; } = 0; //	Hour of change to daylight saving time
        public byte DaylightStartMinute { get; set; } = 0; //	Minute of change to daylight saving time
        public byte StandardStartMonth { get; set; } = 0; //	Month of change to standard time
        public byte StandardStartWeek { get; set; } = 0; //	Week of change to standard time: 1= 1st occurrence of the weekday in the month, ..., 5= last occurrence
        public byte StandardStartWeekday { get; set; } = 0; //	Weekday of change to standard time
        public byte StandardStartHour { get; set; } = 0; //	Hour of change to standard time
        public byte StandardStartMinute { get; set; } = 0; //	Minute of change to standard time

        [StringLength(80)]
        public string TimeZoneName { get; set; } //	Name of the used time zone like in Windows XP: "(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna"
    }

```

## Use the Serializer

The following sample shows how you can use the serializer to serialize and deserialize data structures.
```cs
var serializer = new PlcDataMapperSerializer();
var numberOfBytesToRead = serializer.SerializedByteSize<TimeTransformationRule>();

// read the data from the plc  e.g. with: 
IEnumerable<DataValue> results = await _dacs7.ReadAsync(ReadItem.Create<byte[]>("DB100", 0, (ushort)numberOfBytesToRead));

if(results.FirstOrDefault() is DataValue dv && dv.IsSuccessReturnCode)
{
    // deserialize the data from the byte array
    var rule = serializer.Deserialize<TimeTransformationRule>(result.Data);

    // change the data 
    rule.DaylightStartWeekday = 2;

    // serialize the class to a byte array
    var writeBack = serializer.Serialize(rule);

    // write it back
    _dacs7.WriteAsync(WriteItem.Create("DB100", 0, writeBack));
}

```