

using Papper.Attributes;
using System;

namespace Papper.Tests.Mappings.ChargenRV
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

    

    public class UDT_ChargenRV_Data_Element_CntPiecePerUnit
    {
        
        [StringLength(6)]
        public string SDesc1 { get; set; }	//Kurzbezeichnung
        
        [StringLength(9)]
        public string Data1 { get; set; }	//Daten
        
        [StringLength(6)]
        public string SDesc2 { get; set; }	//Kurzbezeichnung
        
        [StringLength(9)]
        public string Data2 { get; set; }	//Daten
        public Int16 Action { get; set; }
        public Int16 Count { get; set; }
        public Int16 Signal_Out { get; set; }
    }

    

    public class UDT_ChargenRV_Data_Element
    {
        public bool Inconsistent { get; set; }	//Eintrag inkonsistent
        public bool Deactivated { get; set; }	//Eintrag deaktiviert -> wird an diesem Platz nicht verbaut
        public Int16 InputAdrTrayChange { get; set; }	//Eingangsadresse für Behälterwechsel
        public Int16 InputAdrBitTrayChange { get; set; }	//Eingangsadresse Bit für Behälterwechsel
        public Int16 MaxCntPiecesInTray { get; set; }	//Stück im Behälter
        [ReadOnly(true)]
        public Int16 ActCntPieces { get; set; }	//Aktuelle Stückzahl
        [ReadOnly(true)]
        public Int16 ActCntPiecePerUnit { get; set; }	//Aktuelle Stückzahl verbaute Stück pro Einheit (nur Ausgabe)

        [ArrayBounds(1,8,0)]
        public UDT_ChargenRV_Data_Element_CntPiecePerUnit[] CntPiecePerUnit { get; set; }	//verbaute Stückzahl je Einheit (Motor)
        
        [StringLength(4)]
        public string SDesc { get; set; }	//Bauteil Kurzbezeichnung

        [ArrayBounds(1,9,0)]
        public Char[] Partnumber { get; set; }	//Sachnummer
        
        [StringLength(40)]
        public string ScanData { get; set; }	//gescannte Daten
    }

    

    public class UDT_ChargenRV_Data
    {
        [ArrayBounds(1,50,0)]
        public UDT_ChargenRV_Data_Element[] Element { get; set; }
    }

    

    public class UDT_ChargenRV_Results_Element
    {
        public UDT_DatenErgeb_Univ UnivResult { get; set; }
    }

    

    public class UDT_ChargenRV_Results
    {
        [ArrayBounds(1,50,0)]
        public UDT_ChargenRV_Results_Element[] Element { get; set; }
    }

    

    public class UDT_ChargenRV_PlcVars_ElemList
    {
        public Int16 LastElem { get; set; }

        [ArrayBounds(1,50,0)]
                
        [StringLength(4)]
        public string[] SDesc { get; set; }

        [ArrayBounds(1,50,0)]
        public Int16[] Index { get; set; }

        [ArrayBounds(1,50,0)]
        public bool[] IpmFinished { get; set; }

        [ArrayBounds(1,50,0)]
        public bool[] DeselectedByUniv { get; set; }
    }

    

    public class UDT_ChargenRV_PlcVars_Univ_InitStruct
    {
        
        [StringLength(6)]
        public string Desc1 { get; set; }
        
        [StringLength(9)]
        public string Data1 { get; set; }
        
        [StringLength(6)]
        public string Desc2 { get; set; }
        
        [StringLength(9)]
        public string Data2 { get; set; }
        public Int16 Action { get; set; }
        public Int16 Count { get; set; }
        public Int16 Signal_Out { get; set; }
    }

    

    public class UDT_ChargenRV_PlcVars_Univ
    {
        public UDT_ChargenRV_PlcVars_Univ_InitStruct InitStruct { get; set; }	//Muss leer bleiben zum initialisieren
        public UDT_DatenAusw_Univ TempEval { get; set; }
    }

    

    public class UDT_ChargenRV_PlcVars
    {
        public Int16 ClearLineNo { get; set; }	//Zeilen Nummer zum löschen
        public bool ScanRequired { get; set; }	//Scan erforderlich
        public bool Finished { get; set; }	//Komplett fertig
        public bool HM_FL_WpcIn { get; set; }
        public bool HM_FL_NewScanData { get; set; }
        public bool HM_FL_Start { get; set; }
        public bool ErrIpmSlotsFull { get; set; }	//IPM Sendefächer voll
        public bool ErrEntry { get; set; }	//Fehler bei einem Eintrag vorhanden

        [ArrayBounds(1,50,0)]
        public bool[] HM_FLP_InputTrayChange { get; set; }

        [ArrayBounds(1,50,0)]
        public bool[] HM_FLN_InputTrayChange { get; set; }
        public UDT_ChargenRV_PlcVars_ElemList ElemList { get; set; }
        public UDT_ChargenRV_PlcVars_Univ Univ { get; set; }
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

        [ArrayBounds(1,24,0)]
        public UDT_DatenAusw_Univ_Ausw[] Ausw { get; set; }	//24 Auswertungen

    }

    
    public class UDT_ChargenRV
    {
        public UDT_ChargenRV_Data Data { get; set; }
        public UDT_ChargenRV_Results Results { get; set; }
        public UDT_ChargenRV_PlcVars PlcVars { get; set; }

    }

    [Mapping("DB_BST3_ChargenRV", "DB3018", 0)]
    public class DB_BST3_ChargenRV
    {
        public UDT_ChargenRV Dat { get; set; }

    }

}

