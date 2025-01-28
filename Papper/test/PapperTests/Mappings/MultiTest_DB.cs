using Papper.Attributes;
using System;

namespace Papper.Tests.Mappings
{
    
    

    [PlcType("MultiTest_DB.Storage")]
    public class MultiTest_DB_Storage
    {
        public byte Column { get; set; }
        public byte Row { get; set; }
        public byte Status { get; set; }
        public byte Status_CP { get; set; }
        public DateTime Storage_at_time { get; set; }

        [ArrayBounds(1,8,0)]
        public Char[] CH_serial_No { get; set; }

        [ArrayBounds(1,5,0)]
        public Char[] Res1 { get; set; }

        [ArrayBounds(1,9,0)]
        public Char[] CH_Type { get; set; }

        [ArrayBounds(1,4,0)]
        public byte[] Res12 { get; set; }
    }


    [Mapping("MultiTest_DB", "DB465", 0)]
    public class MultiTest_DB
    {

        [ArrayBounds(1,32,0)]
        [ArrayBounds( 1,9,1)]
        public MultiTest_DB_Storage[][] Storage { get; set; }

    }


}

