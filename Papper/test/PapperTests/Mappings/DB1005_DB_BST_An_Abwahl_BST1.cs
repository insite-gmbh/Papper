

using Papper.Attributes;
using System;

namespace Papper.Tests.Mappings.BstAbw
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

    

    public class UDT_BST_AN_Abwahl_An_Abwahl
    {
        public bool aktiviert { get; set; }	//Zeile aktiviert
        public UDT_DatenAusw_Univ UNIV_Auswertung { get; set; }

        [ReadOnly(true)]
        public UDT_DatenErgeb_Univ UNIV_Ergebnis { get; set; }
    }

    

    public class UDT_BST_AN_Abwahl_Bearbeiten
    {
        
        [StringLength(8)]
        public string Herkunft { get; set; }	//Herkunfts Station kann auch Wildcard * sein
        
        [StringLength(8)]
        public string Ziel { get; set; }	//Ziel Station kann auch Wildcard * sein
        public bool aktiviert { get; set; }	//Zeile aktiviert
        public bool Irrlaeufer { get; set; }	//Wenn dieses Bit aktiv ist, dann gillt der Motor als Irrläufer
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

    
    public class UDT_BST_AN_Abwahl
    {
        public UDT_BST_AN_Abwahl_An_Abwahl An_Abwahl { get; set; }	//An.- Abwahl Arbeitsplatz

        [ArrayBounds(1,6,0)]
        public UDT_BST_AN_Abwahl_Bearbeiten[] Bearbeiten { get; set; }
        
        [StringLength(8)]
        public string Ausw_Stationsname { get; set; }	//Derzeit Ausgewählte Station (Verwendung nur WinCCflex Intern)

    }

    [Mapping("DB_BST_An_Abwahl_BST1", "DB1005", 0)]
    public class DB_BST_An_Abwahl_BST1
    {
        public UDT_BST_AN_Abwahl BST { get; set; }

    }

}

