

using Papper.Attributes;
using System;

namespace Insite.Customer.Data.DB_BST1_Regal_1_Konfig
{



    public class UDT_DatenAusw_Univ_Ausw
    {

        [StringLength(6)]
        public string Bez1 { get; set; }

        [StringLength(9)]
        public string Data1 { get; set; }

        [StringLength(6)]
        public string Bez2 { get; set; }

        [StringLength(9)]
        public string Data2 { get; set; }
        public Int16 Aktion { get; set; }
        public Int16 Anzahl { get; set; }
        public Int16 Signal_Auslauf { get; set; }
    }



    public class UDT_Regal_Fach
    {
        public bool aktiv { get; set; }	//Fach aktiviert
        [ReadOnly(true)]
        public bool Out { get; set; }	//Zustand Ausgang
        [ReadOnly(true)]
        public bool fertig { get; set; }	//Fertigmeldung Fach
        public bool angew { get; set; }	//Fach angewaehlt
        [ReadOnly(true)]
        public bool HIO { get; set; }	//M-ID: Hand-IO
        [ReadOnly(true)]
        public bool ABGW { get; set; }	//M-ID: Montageschritt abgewählt
        public bool nur_Anzeige { get; set; }	//Fach wird nur zur Anzeige verwendet -> keine Eingriffskontrolle
        public Int16 M_ID { get; set; }	//Montageschritt-ID
    }



    public class UDT_Regal_UNIV_Fach_Ausw
    {
        public UDT_DatenAusw_Univ Auswertungen { get; set; }
    }



    public class UDT_Regal_UNIV_Fach_Ergeb
    {
        [ReadOnly(true)]
        public UDT_DatenErgeb_Univ Data { get; set; }
    }



    public class UDT_DatenErgeb_Univ
    {
        [ReadOnly(true)]
        public Int16 IO_Nr { get; set; }	//Nr der gefundenen Auswertung
        [ReadOnly(true)]
        public Int16 Fehler_Nr { get; set; }	//Fehler Nummer
        [ReadOnly(true)]
        public Int16 Fehler_Position { get; set; }	//Zeilen Position des Fehlers
        [ReadOnly(true)]

        [StringLength(80)]
        public string Fehler_Text { get; set; }	//Fehler Text

    }


    public class UDT_DatenAusw_Univ
    {

        [ArrayBounds(1, 24, 0)]
        public UDT_DatenAusw_Univ_Ausw[] Ausw { get; set; }	//24 Auswertungen

    }


    public class UDT_Regal
    {
        public bool vorhanden { get; set; }	//Regal ist vorhanden (PLC)
        [ReadOnly(true)]
        public bool fertig { get; set; }	//Fertigmeldung Gesamt Regal
        public bool aktiv { get; set; }	//Regal aktiv
        [ReadOnly(true)]
        public bool Hand_IO { get; set; }	//Regal Hand IO

        [ArrayBounds(1, 8, 0)]
        public UDT_Regal_Fach[] Fach { get; set; }

        [StringLength(20)]
        public string Headline { get; set; }	//editierbares Beschreibungsfeld im Bildkopf

        [ArrayBounds(1, 8, 0)]
        public UDT_Regal_UNIV_Fach_Ausw[] UNIV_Fach_Ausw { get; set; }	//Universal Auswertung
        [ReadOnly(true)]

        [ArrayBounds(1, 8, 0)]
        public UDT_Regal_UNIV_Fach_Ergeb[] UNIV_Fach_Ergeb { get; set; }	//ergebnisse der Universal Auswertung

    }

    [Mapping("DB_BST1_Regal_1_Konfig", "DB1035", 0)]
    public class DB_BST1_Regal_1_Konfig
    {
        public UDT_Regal Regal { get; set; }

    }

}

