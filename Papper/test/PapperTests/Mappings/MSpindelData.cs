using Papper.Attributes;
using System;
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1819 // Properties should not return arrays
namespace Papper.Tests.Mappings
{
    public class UDT_IDATInterface_PLCtoIDAT
    {
        public bool WriteEnable { get; set; }
        public bool UpdateRequired { get; set; }
    }


    public class UDT_IDATInterface_IDATtoPLC
    {
        public bool DataWritten { get; set; }
        public bool UpdateRequiredDetected { get; set; }
        public bool ErrorOccurred { get; set; }
        public bool Toggle { get; set; }
        public byte ErrorCode { get; set; }
    }

    public class UDT_IDATInterface
    {
        public UDT_IDATInterface_PLCtoIDAT PLCtoIDAT { get; set; }
        public UDT_IDATInterface_IDATtoPLC IDATtoPLC { get; set; }
        public DateTime CurrentTimeFromPLC { get; set; }
        public DateTime LastWriteTimeFromIDAT { get; set; }
        public DateTime TimeToCopyFromXLS { get; set; }

        [StringLength(8)]
        public string MonrToCopyFromXLS { get; set; }
        public bool CopyNowFromXLS { get; set; }

    }
    public class UDT_Programs_MSpindle_Component_Programe
    {
        [ArrayBounds(1, 2, 0)]
        public char[] Prg_Name { get; set; }    //ProgramName
        public short Cnt_Tightenings { get; set; }  //Count TightenProgs
    }


    public class UDT_Programs_MSpindle_Component
    {

        [StringLength(60)]
        public string CompTitle { get; set; }   //title
        public short Channel_No { get; set; }   //Number from TighteningChannels
        public short Slot { get; set; } //SocketNumber
        public short TighteningSequence { get; set; }   //TighteningSequence
        public short SF { get; set; }   //TightenProg for Socket
        public short unTightenTightenProg { get; set; } //TightenProg release Global

        [ArrayBounds(1, 64, 0)]
        public UDT_Programs_MSpindle_Component_Programe[] Programe { get; set; }
    }



    public class UDT_Programs_MSpindle
    {

        [ArrayBounds(1, 99, 0)]
        public UDT_Programs_MSpindle_Component[] Component { get; set; }

    }

    [Mapping("DB_BST1_IDAT_MSpindData", "DB1144", 0)]
    [Mapping("DB_BST2_IDAT_MSpindData", "DB2144", 0)]
    [Mapping("DB_BST3_IDAT_MSpindData", "DB3144", 0)]
    [Mapping("DB_BST4_IDAT_MSpindData", "DB4144", 0)]
    [Mapping("DB_BST5_IDAT_MSpindData", "DB5144", 0)]
    [Mapping("DB_BST6_IDAT_MSpindData", "DB6144", 0)]
    public class MSpindData
    {
        [StringLength(8)]
        public string SDesc { get; set; }
        public bool active { get; set; }
        public UDT_Programs_MSpindle Config { get; set; }

    }

    [Mapping("DB_IDAT_MSpindleData", "DB241", 0)]
    [Mapping("DB_IDAT_MSpindleData1", "DB242", 0)]
    public class MSpindleInterface
    {
        public UDT_IDATInterface IDATInterface { get; set; }

    }



}
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1819 // Properties should not return arrays