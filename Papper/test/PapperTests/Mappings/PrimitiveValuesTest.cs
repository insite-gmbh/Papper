using Papper.Attributes;

namespace Papper.Tests.Mappings
{
    [Mapping("PrimitiveValuesMapping", "DB99", 0)]
    public class PrimitiveValuesMapping
    {
        public ushort UInt16 { get; set; }
        public short Int16 { get; set; }
        public uint UInt32 { get; set; }
        public int Int32 { get; set; }
        public float Single { get; set; }
        public char Char { get; set; }
    }
}
