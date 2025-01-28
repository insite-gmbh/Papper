

using Papper.Attributes;
using System;

namespace Papper.Tests.Mappings.DeviceConfig
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

    

    public class UDT_Geraete_IN
    {
        public bool IN { get; set; }	//Zustand Eingang
        public bool fertig { get; set; }	//Fertigmeldung Gerät
        public bool angew { get; set; }	//Gerät angewählt
        public bool HIO { get; set; }	//M-ID: Hand-IO
        public bool ABGW { get; set; }	//M-ID: Montageschritt abgewählt
        public bool aktiv { get; set; }	//Gerät aktiv
        public bool Signal_1_fuer_Einlauf { get; set; }	//In muss Signal 1 führen, damit der Vorstopper öffnet
        public Int16 M_ID { get; set; }	//Montageschritt-ID
        
        [StringLength(10)]
        public string Text { get; set; }
    }

    

    public class UDT_Geraete_OUT
    {
        public bool OUT { get; set; }	//Zustand Ausgang
        public bool aktiv { get; set; }	//Ausgang aktiv
        public bool angew { get; set; }	//ohne Funktion
        public bool test { get; set; }	//Test: Ausgang ansteuern
        public bool Out_bei_WT_in { get; set; }	//Ausgang erst bei WT in Position ansteuern
        
        [StringLength(20)]
        public string Text { get; set; }
    }

    

    public class UDT_Geraete_UNIV_IN_Ausw
    {
        public UDT_DatenAusw_Univ Auswertungen { get; set; }
    }

    

    public class UDT_Geraete_UNIV_IN_Ergeb
    {
        public UDT_DatenErgeb_Univ Data { get; set; }
    }

    

    public class UDT_Geraete_UNIV_OUT_Ausw
    {
        public UDT_DatenAusw_Univ Auswertungen { get; set; }
    }

    

    public class UDT_Geraete_UNIV_OUT_Ergeb
    {
        public UDT_DatenErgeb_Univ Data { get; set; }
    }


    
    public class UDT_DatenAusw_Univ
    {

        [ArrayBounds(1,24,0)]
        public UDT_DatenAusw_Univ_Ausw[] Ausw { get; set; }	//24 Auswertungen

    }

    
    public class UDT_DatenErgeb_Univ
    {
        public Int16 IO_Nr { get; set; }	//Nr der gefundenen Auswertung
        public Int16 Fehler_Nr { get; set; }	//Fehler Nummer
        public Int16 Fehler_Position { get; set; }	//Zeilen Position des Fehlers
        
        [StringLength(80)]
        public string Fehler_Text { get; set; }	//Fehler Text

    }

    
    public class UDT_Geraete
    {
        [ReadOnly(true)]
        public bool vorhanden { get; set; }	//Regal ist vorhanden (PLC)
        [ReadOnly(true)]
        public bool fertig { get; set; }	//Fertigmeldung Gesamt Regal
        [ReadOnly(true)]
        public bool aktiv { get; set; }	//Regal aktiv
        [ReadOnly(true)]
        public bool Hand_IO { get; set; }	//Hand IO Eingänge Gesamtende

        [ArrayBounds(1,4,0)]
        public UDT_Geraete_IN[] IN { get; set; }	//Eingänge

        [ArrayBounds(1,4,0)]
        public UDT_Geraete_OUT[] OUT { get; set; }	//Ausgänge

        [ArrayBounds(1,4,0)]
        public UDT_Geraete_UNIV_IN_Ausw[] UNIV_IN_Ausw { get; set; }	//Universal Auswertung
        [ReadOnly(true)]

        [ArrayBounds(1,4,0)]
        public UDT_Geraete_UNIV_IN_Ergeb[] UNIV_IN_Ergeb { get; set; }	//ergebnisse der Universal Auswertung

        [ArrayBounds(1,4,0)]
        public UDT_Geraete_UNIV_OUT_Ausw[] UNIV_OUT_Ausw { get; set; }	//Universal Auswertung
        [ReadOnly(true)]

        [ArrayBounds(1,4,0)]
        public UDT_Geraete_UNIV_OUT_Ergeb[] UNIV_OUT_Ergeb { get; set; }	//ergebnisse der Universal Auswertung

    }

    [Mapping("DB_BST1_Geraete_1_Konfig", "DB1025", 0)]
    public class DB_BST1_Geraete_1_Konfig
    {
        public UDT_Geraete Geraete { get; set; }

    }

}

