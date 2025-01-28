using Papper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Papper.Tests.Mappings
{
    [Mapping("SampleData", "DB909", 0)]
    public class SampleData
    {
        [ReadOnly(true)]
        public UInt16 UInt16 { get; set; }
        public Int16 Int16 { get; set; }
        public UInt32 UInt32 { get; set; }
        public Int32 Int32 { get; set; }
        public Single Single { get; set; }
        public char Char { get; set; }

        public bool Bit1 { get; set; }

        [ReadOnly(true)]
        public bool Bit2 { get; set; }
        public bool Bit3 { get; set; }
        public bool Bit4 { get; set; }
        public bool Bit5 { get; set; }
        public bool Bit6 { get; set; }
        public bool Bit7 { get; set; }
        public bool Bit8 { get; set; }
    }


    [Mapping("SampleDataSubstruct", "DB989", 0)]
    public class SampleDataSubstruct
    {
        [ReadOnly(true)]
        public UInt16 UInt16 { get; set; }
        public SampleDataSubstruct1 SubStruct { get; set; }
        public SampleDataSubstruct2 SubStruct2 { get; set; }
    }

    public class SampleDataSubstruct1
    {
        [ReadOnly(true)]
        public UInt16 UInt16 { get; set; }
        public Int16 Int16 { get; set; }
    }

    public class SampleDataSubstruct2
    {
        public UInt16 UInt16 { get; set; }
        public Int16 Int16 { get; set; }
    }


    [Mapping("SampleDataAccessNames", "DB910", 0)]
    public class SampleDataAccessNames
    {
        [ReadOnly(true)]
        public UInt16 UInt16 { get; set; }

        [NotAccessible(true)]
        public Int16 Int16 { get; set; }

        [SymbolicAccessName("TestXX")]
        public UInt32 UInt32 { get; set; }
        public Int32 Int32 { get; set; }
        public Single Single { get; set; }
        public char Char { get; set; }

        public bool Bit1 { get; set; }

        [ReadOnly(true)]
        public bool Bit2 { get; set; }
        public bool Bit3 { get; set; }
        public bool Bit4 { get; set; }
        public bool Bit5 { get; set; }
        public bool Bit6 { get; set; }
        public bool Bit7 { get; set; }
        public bool Bit8 { get; set; }
    }


    [Mapping("SAFE_SEQ_FSDB", "DB10", 0)]
    public class SAFE_SEQ_FSDB
    {
        [AliasName("ST001+OC001 RELEASE")]
        [SymbolicAccessName("ST001+OC001 RELEASE")]
        public bool ST001_OC001_RELEASE { get; set; }	//TRUE = Element IO Release Panel OC001 Not-Halt
        [AliasName("ST001+OZ100-SF03")]
        [SymbolicAccessName("ST001+OZ100-SF03")]
        public bool ST001_OZ100_SF03 { get; set; }	//TRUE = Element IO Release Zweihand Not-Halt
        [AliasName("ST001+RT001 RELEASE")]
        [SymbolicAccessName("ST001+RT001 RELEASE")]
        public bool ST001_RT001_RELEASE { get; set; }	//TRUE = Element IO Release Rolltor RT001; Antriebe können fahren
        public bool OP010 { get; set; }	//TRUE = Element IO
        [AliasName("OP010-RELEASE")]
        [SymbolicAccessName("OP010-RELEASE")]
        public bool OP010_RELEASE { get; set; }	//Zustimmtaster
        public bool ESTOP_GROUP1 { get; set; }	//Not-Halt Gruppe
        public bool GATE_GROUP1 { get; set; }	//Schutztür Gruppe
        public bool RELEASE_ZWEIHAND_START { get; set; }	//TRUE = Element IO Release Achsen X und Motor
        [AliasName("RELEASE_ST001+SD101")]
        [SymbolicAccessName("RELEASE_ST001+SD101")]
        public bool RELEASE_ST001_SD101 { get; set; }	//TRUE = Element IO Release Schutztür SD101 entriegelt
        [AliasName("ACKREQ_ST001+SD101")]
        [SymbolicAccessName("ACKREQ_ST001+SD101")]
        public bool ACKREQ_ST001_SD101 { get; set; }
        [AliasName("RELEASE_ST001+SD100")]
        [SymbolicAccessName("RELEASE_ST001+SD100")]
        public bool RELEASE_ST001_SD100 { get; set; }	//TRUE = Element IO Release Schutztür SD100 entriegelt
        [AliasName("ACKREQ_ST001+SD100")]
        [SymbolicAccessName("ACKREQ_ST001+SD100")]
        public bool ACKREQ_ST001_SD100 { get; set; }

    }
}
