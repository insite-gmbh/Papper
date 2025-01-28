using Papper.Attributes;

namespace Papper.Tests.Mappings
{
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    public class RawUpdateDataRequest
    {
        public static readonly int SizeInBytes = 48;
        [ArrayBounds(1, 16, 0)]
        public char[] Station { get; set; }
        [ArrayBounds(1, 20, 0)]
        public char[] Serialnumber { get; set; }
        [ArrayBounds(1, 10, 0)]
        public char[] Functionname { get; set; }
        public ushort CountDataPoints { get; set; }

        [Ignore()]
        public RawDataPoint[] DataPoints { get; set; }
    }

    public class RawDataPoint
    {

        public static readonly int SizeInBytes = 96;
        public bool Valid { get; set; }
        public bool OkNok { get; set; }
        [ArrayBounds(1, 20, 0)]
        public char[] Step { get; set; }
        [ArrayBounds(1, 20, 0)]
        public char[] Designation { get; set; }
        [ArrayBounds(1, 20, 0)]
        public char[] DesignationValue { get; set; }
        [ArrayBounds(1, 6, 0)]
        public char[] Type { get; set; }
        [ArrayBounds(1, 20, 0)]
        public char[] ValString { get; set; }
        public int ValLong { get; set; }
        public float ValFloat { get; set; }
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
    }

}

