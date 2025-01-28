using Papper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papper.Tests.Mappings
{


    namespace Insite.Customer.Data.DATA_ARCHIVE_DB
    {



        [PlcType("DATA_VERW_ITEM_UDT.ST165")]
        public class DATA_VERW_ITEM_UDT_ST165
        {
            [PlcType("DateTimeL")] public DateTime ZEITP_IN { get; set; }   //Zeitpunkt an dem das Bauteil übernommen wird
            public Int16 PLATZ_NR_IN { get; set; }  //Übergabeplatz

            [PlcType("DateTimeL")] public DateTime ZEITP_OUT { get; set; }  //Zeitpunkt an dem das Bauteil abbegeben wurde
            public Int16 PLATZ_NR_OUT { get; set; } //Übergabeplatz
        }



        [PlcType("DATA_VERW_ITEM_UDT.ST170_171")]
        public class DATA_VERW_ITEM_UDT_ST170_171
        {
            [PlcType("DateTimeL")] public DateTime ZEITP_IN { get; set; }   //Einlegen des Bauteil in den Vorwärmofen

            [PlcType("DateTimeL")] public DateTime ZEITP_OUT { get; set; }  //Entnahme des Bauteil aus den Vorwärmofen

            [StringLength(8)]
            public string BEARBEITUNGSOFEN { get; set; }    //Bauteil wurde in diesem Ofen bearbeitet
            public TimeSpan ZEIT_PROZESS { get; set; }  //Prozesszeit vom Bausteil bei der entnahme
            public TimeSpan ZEIT_PROZESS_MIN { get; set; }  //Zeit Prozess unterer Grenze
            public TimeSpan ZEIT_PROZESS_MAX { get; set; }  //Zeit Prozess oberere Grenze
            public bool ZEIT_PROZESS_IO { get; set; }   //Prozess Zeit in Ordnung
            public bool ZEIT_PROZESS_NIO { get; set; }  //Prozess Zeit in nicht Ordnung
            public Single TEMP { get; set; }    //Prozesstemperatur
            public Single TEMP_MIN { get; set; }    //Temperatur Prozess unterer Grenze
            public Single TEMP_MAX { get; set; }    //Temperatur Prozess oberere Grenze
            public bool TEMP_IO { get; set; }   //Prozess Temperatur in Ordnung
            public bool TEMP_NIO { get; set; }  //Prozess Temperatur in nicht Ordnung
            public Single GEWICHT { get; set; } //Gewicht des Bauteils
            public Single GEWICHT_MIN { get; set; } //Minimales Gewicht
            public Single GEWICHT_MAX { get; set; } //Maximales gewicht
            public bool GEWICHT_IO { get; set; }    //Gewicht des Bauteils IO
            public bool GEWICHT_NIO { get; set; }   //Gewicht des Bauteils niO
            public bool GEWICHT_WERKER_IO { get; set; } //Gewicht des Bauteils niO durch Werker
            public bool GEWICHT_WERKER_NIO { get; set; }    //Gewicht des Bauteils iO durch Werker
            public Int16 PLATZ_NR { get; set; } //Ausgewerteter Platz des Ofen
        }



        [PlcType("DATA_VERW_ITEM_UDT.FT171")]
        public class DATA_VERW_ITEM_UDT_FT171
        {
            [PlcType("DateTimeL")] public DateTime ZEITP_IN { get; set; }   //Zeitpunkt an dem das Bauteil übernommen wird
            public Single TEMP { get; set; }    //Temperatur beim aufnehmen
            public Single TEMP_AUSSCHUSS { get; set; }  //Temperatur Ausschuss
            public Single TEMP_DOPPEL { get; set; } //Temperatur Doppelt härten
            public bool TEMP_IO { get; set; }   //Prozess Temperatur in Ordnung
            public bool TEMP_NIO_DOPPEL { get; set; }   //Prozess Temperatur in nicht Ordnung (Doppelt härten)
            public bool TEMP_NIO_AUSSCHUSS { get; set; }    //Prozess Temperatur in nicht Ordnung (Auschuss)
            public Single GEWICHT { get; set; } //Gewicht des Bauteils
            public Single GEWICHT_MIN { get; set; } //Minimales Gewicht
            public Single GEWICHT_MAX { get; set; } //Maximales gewicht
            public bool GEWICHT_IO { get; set; }    //Gewicht des Bauteils IO
            public bool GEWICHT_NIO { get; set; }   //Gewicht des Bauteils niO
            public bool GEWICHT_WERKER_IO { get; set; } //Gewicht des Bauteils niO durch Werker
            public bool GEWICHT_WERKER_NIO { get; set; }    //Gewicht des Bauteils iO durch Werker
            public Single GEWICHT_DIFF { get; set; }    //Differenz gewicht vor und nach  VAG
            public Single GEWICHT_DIFF_MIN { get; set; }    //Minimales Gewicht
            public Single GEWICHT_DIFF_MAX { get; set; }    //Maximales gewicht
            public bool GEWICHT_DIFF_IO { get; set; }   //Gewicht des Bauteils IO
            public bool GEWICHT_DIFF_NIO { get; set; }  //Gewicht des Bauteils niO
            public bool GEWICHT_DIFF_WERKER_IO { get; set; }    //Gewicht des Bauteils niO durch Werker
            public bool GEWICHT_DIFF_WERKER_NIO { get; set; }   //Gewicht des Bauteils iO durch Werker
            public Single GEWICHT_OFFS_MIN_WIEDERH { get; set; }    //Minimales Gewicht Wiederholer (ohne Füllung)
            public Single GEWICHT_OFFS_MAX_WIEDERH { get; set; }    //Maximales gewicht Wiederholer (ohne Füllung)
            public bool GEWICHT_WIEDERH { get; set; }   //Bauteil ist Wiederholer (ohne Füllung)
        }



        [PlcType("DATA_VERW_ITEM_UDT.ST172_173_174")]
        public class DATA_VERW_ITEM_UDT_ST172_173_174
        {
            [PlcType("DateTimeL")] public DateTime ZEITP_IN { get; set; }   //Zeitpunkt an dem das Bauteil übernommen wird

            [PlcType("DateTimeL")] public DateTime ZEITP_OUT { get; set; }  //Zeitpunkt an dem das Bauteil abbegeben wurde

            [StringLength(8)]
            public string BEARBEITUNGSOFEN { get; set; }    //Bauteil wurde in diesem Ofen bearbeitet
            public TimeSpan ZEIT_PROZESS { get; set; }  //Prozesszeit vom Bausteil bei der entnahme
            public TimeSpan ZEIT_PROZESS_MIN { get; set; }  //Zeit Prozess unterer Grenze
            public TimeSpan ZEIT_PROZESS_MAX { get; set; }  //Zeit Prozess oberere Grenze
            public bool ZEIT_PROZESS_IO { get; set; }   //Prozess Zeit in Ordnung
            public bool ZEIT_PROZESS_NIO { get; set; }  //Prozess Zeit in nicht Ordnung
            public Single TEMP { get; set; }    //Prozesstemperatur
            public Single TEMP_MIN { get; set; }    //Temperatur Prozess unterer Grenze
            public Single TEMP_MAX { get; set; }    //Temperatur Prozess oberere Grenze
            public bool TEMP_IO { get; set; }   //Prozess Temperatur in Ordnung
            public bool TEMP_NIO { get; set; }  //Prozess Temperatur in nicht Ordnung
            public Int16 PLATZ_NR { get; set; } //Ausgewerteter Platz des Ofen
        }



        [PlcType("DATA_VERW_ITEM_UDT.ST175")]
        public class DATA_VERW_ITEM_UDT_ST175
        {
            [PlcType("DateTimeL")] public DateTime ZEITP_IN { get; set; }   //Zeitpunkt an dem das Bauteil übernommen wird

            [PlcType("DateTimeL")] public DateTime ZEITP_OUT { get; set; }  //Zeitpunkt an dem das Bauteil abbegeben wurde
            public TimeSpan ZEIT_PROZESS { get; set; }  //Prozesszeit vom Bausteil bei der entnahme
            public TimeSpan ZEIT_PROZESS_MIN { get; set; }  //Zeit Prozess unterer Grenze
            public TimeSpan ZEIT_PROZESS_MAX { get; set; }  //Zeit Prozess oberere Grenze
            public bool ZEIT_PROZESS_IO { get; set; }   //Prozess Zeit in Ordnung
            public bool ZEIT_PROZESS_NIO { get; set; }  //Prozess Zeit in nicht Ordnung
            public Single TEMP { get; set; }    //Prozesstemperatur
            public Single TEMP_MIN { get; set; }    //Temperatur Prozess unterer Grenze
            public Single TEMP_MAX { get; set; }    //Temperatur Prozess oberere Grenze
            public bool TEMP_IO { get; set; }   //Prozess Temperatur in Ordnung
            public bool TEMP_NIO { get; set; }  //Prozess Temperatur in nicht Ordnung
            public Int16 PLATZ_NR { get; set; } //Ausgewerteter Platz des Ofen
        }



        [PlcType("DATA_VERW_ITEM_UDT.ST176")]
        public class DATA_VERW_ITEM_UDT_ST176
        {
            [PlcType("DateTimeL")] public DateTime ZEITP_IN { get; set; }

            [PlcType("DateTimeL")] public DateTime ZEITP_OUT { get; set; }
            public TimeSpan ZEIT_PROZESS { get; set; }
            public TimeSpan ZEIT_PROZESS_MIN { get; set; }
            public TimeSpan ZEIT_PROZESS_MAX { get; set; }
            public bool ZEIT_PROZESS_IO { get; set; }
            public bool ZEIT_PROZESS_NIO { get; set; }
            public Single TEMP { get; set; }
            public Single TEMP_MIN { get; set; }
            public Single TEMP_MAX { get; set; }
            public bool TEMP_IO { get; set; }
            public bool TEMP_NIO { get; set; }
            public Int16 PLATZ_NR { get; set; }
        }



        [PlcType("DATA_VERW_ITEM_UDT.ST18x")]
        public class DATA_VERW_ITEM_UDT_ST18x
        {

            [StringLength(10)]
            public string STATION { get; set; } //Station in der die Station vergossen wurde
        }



        [PlcType("DATA_VERW_ITEM_UDT.ST19x")]
        public class DATA_VERW_ITEM_UDT_ST19x
        {
            [PlcType("DateTimeL")] public DateTime ZEITP_IN { get; set; }   //Zeitpunkt an dem das Bauteil übernommen wird
            public Int16 WT_NR_IN { get; set; } //WT Nummer transport zu verguss

            [PlcType("DateTimeL")] public DateTime ZEITP_OUT { get; set; }  //Zeitpunkt an dem das Bauteil abbegeben wurde
            public Int16 WT_NR_OUT { get; set; }    //WT Nummer transport zu verguss
        }



        [PlcType("DATA_VERW_ITEM_UDT")]
        public class DATA_VERW_ITEM_UDT
        {
            public bool DS_EMPTY { get; set; }  //Datensatz ist leer
            public bool DS_ARCHIV { get; set; } //Datensatz ist im Archiv, Bauteil nicht mehr in der Anlage
            [AliasName("DMC_A-SEITE")]
            [SymbolicAccessName("DMC_A-SEITE")]

            [StringLength(40)]
            public string DMC_A_Minus_SEITE { get; set; }   //DMC Code des Bausteils
            [AliasName("DMC_B-SEITE")]
            [SymbolicAccessName("DMC_B-SEITE")]

            [StringLength(40)]
            public string DMC_B_Minus_SEITE { get; set; }   //DMC Code des Bausteils

            [StringLength(17)]
            public string PLAN_AUFTRAGS_NR { get; set; }    //Planauftragsnummer des ZB Bauteils
            public Int16 Bank { get; set; } //Bank Nummer

            [StringLength(10)]
            public string SACHNR { get; set; }  //Sachnummer des Bauteils mit Aänderungsindex

            [StringLength(8)]
            public string VARIANTE { get; set; }    //Viantencode Bauteil

            [StringLength(8)]
            public string ORT { get; set; } //Station an der sich das Bauteil befindet

            [StringLength(8)]
            public string ZIEL { get; set; }    //Ziel des Bauteils

            [StringLength(40)]
            public string KOMMENTAR { get; set; }   //Kommentar zum Bauteil
            public Int16 Index { get; set; }    //Index in dem der Datensatz liegt
            public Single SlotPos { get; set; } //Versatz der Klinke in Grad

            [PlcType("DateTimeL")] public DateTime CREATE { get; set; } //Zeitpunk zu dem der Datensatz erzeugt wurde
            public DATA_VERW_ITEM_UDT_ST165 ST165 { get; set; } //Zeitpunk an dem das Bauteil von der ST165 übernommen wurde
            public DATA_VERW_ITEM_UDT_ST170_171 ST170_171 { get; set; } //Vorwärmofen
            public DATA_VERW_ITEM_UDT_FT171 FT171 { get; set; } //Portal Abgage
            public DATA_VERW_ITEM_UDT_ST172_173_174 ST172_173_174 { get; set; } //Aushärteofen
            public DATA_VERW_ITEM_UDT_ST175 ST175 { get; set; } //Kühler
            public DATA_VERW_ITEM_UDT_ST176 ST176 { get; set; }
            public DATA_VERW_ITEM_UDT_ST18x ST18x { get; set; } //Daten zum Verguss
            public DATA_VERW_ITEM_UDT_ST19x ST19x { get; set; } //Bauteil zu und von R03R11

        }

        [Mapping("DATA_ARCHIVE_DB", "DB98", 0)]
        public class DATA_ARCHIVE_DB
        {

            [ArrayBounds(1, 20, 0)]
            public DATA_VERW_ITEM_UDT[] DATA { get; set; }
            public bool FP_Transfer_FT170 { get; set; }
            public bool FP_Transfer_FT171 { get; set; }
            public bool FP_Transfer_FT172 { get; set; }
            public bool PartTransfer { get; set; }

        }

    }


}
