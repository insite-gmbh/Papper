using Papper.Attributes;

#pragma warning disable CA1819 // Properties should not return arrays
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Papper.Tests.Mappings
{
    [Mapping("ARRAY_TEST_MAPPING_1", "DB21", 0)]
    [Mapping("ARRAY_TEST_MAPPING_2", "DB22", 0)]
    [Mapping("ARRAY_TEST_MAPPING_3", "DB23", 0)]
    [Mapping("ARRAY_TEST_MAPPING_4", "DB24", 0)]
    [Mapping("ARRAY_TEST_MAPPING_5", "DB25", 0)]
    public class ArrayTestMapping
    {
        [ArrayBounds(1, 50000, 0)]

        public byte[] BigByteArray { get; set; }


        [ArrayBounds(1, 50000, 0)]
        public char[] BigCharArray { get; set; }

        [ArrayBounds(1, 5000, 0)]
        public int[] BigIntArray { get; set; }

        [ArrayBounds(1, 10, 0)]
        public byte[] ByteElements { get; set; }

        [ArrayBounds(1, 10, 0)]
        public char[] CharElements { get; set; }

        [ArrayBounds(1, 10, 0)]
        public int[] IntElements { get; set; }
    }


        [Mapping("DB_TestCTT", "DB4", 0)]
        public class DB_TestCTT
        {
            public bool StartTrack { get; set; }
            public bool StopTrack { get; set; }
            public bool StartEinlauf { get; set; }
            public bool StopEinlauf { get; set; }
            public bool StartBearbeitung { get; set; }
            public bool StopBearbeitung { get; set; }
            public bool StartAuslauf { get; set; }
            public bool StopAuslauf { get; set; }

        }

}



#pragma warning restore CA1819 // Properties should not return arrays
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.