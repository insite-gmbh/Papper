

using Papper.Attributes;
using System;

namespace Papper.Tests.Mappings
{
    
    

    [PlcType("UDT_StorageData.Status")]
    public class UDT_StorageData_Status
    {
        public bool belegt { get; set; }
        public bool reorg { get; set; }
        public bool inkonsistent { get; set; }	//Daten RF <-> DB ungleich
        public bool gesperrt { get; set; }	//manuell gesperrt
        public bool autom_gesperrt { get; set; }	//automatisch gesperrt
        public bool Leerpalette { get; set; }	//WT ist leer
        public bool Repapalette { get; set; }	//Wt soll zu Repa
        public bool FL_gesperrt { get; set; }	//Flanke Fach gesperrt
        public bool in_Batch { get; set; }	//BT in Batch
    }

    

    [PlcType("DB_ZK_Storage_BandG.akt_Einlagern.gasse")]
    public class DB_ZK_Storage_BandG_akt_Einlagern_gasse
    {
        [ArrayBounds(1,10,0)]
        public Char[] LPNR { get; set; }

        [ArrayBounds(1,10,0)]
        public Char[] Sach_Nr { get; set; }

        [ArrayBounds(1,8,0)]
        public Char[] Ser_Nr { get; set; }	//Zylinderkopf Seriennummer
        public bool Leerpalette { get; set; }
        public bool Repapalette { get; set; }
    }

    

    [PlcType("DB_ZK_Storage_BandG.akt_Einlagern")]
    public class DB_ZK_Storage_BandG_akt_Einlagern
    {
        [ArrayBounds(1,2,0)]
        public DB_ZK_Storage_BandG_akt_Einlagern_gasse[] gasse { get; set; }
    }

    

    [PlcType("DB_ZK_Storage_BandG.akt_Auslagern.gasse")]
    public class DB_ZK_Storage_BandG_akt_Auslagern_gasse
    {
        [ArrayBounds(1,10,0)]
        public Char[] LPNR { get; set; }

        [ArrayBounds(1,10,0)]
        public Char[] Sach_Nr { get; set; }

        [ArrayBounds(1,8,0)]
        public Char[] Ser_Nr { get; set; }	//Zylinderkopf Seriennummer
    }

    

    [PlcType("DB_ZK_Storage_BandG.akt_Auslagern")]
    public class DB_ZK_Storage_BandG_akt_Auslagern
    {
        [ArrayBounds(1,2,0)]
        public DB_ZK_Storage_BandG_akt_Auslagern_gasse[] gasse { get; set; }
    }

    

    [PlcType("DB_ZK_Storage_BandG.Sonderauslagern")]
    public class DB_ZK_Storage_BandG_Sonderauslagern
    {
        [ArrayBounds(1,10,0)]
        public Char[] LPNR { get; set; }

        [ArrayBounds(1,10,0)]
        public Char[] Sach_Nr { get; set; }
        public Int16 Gasse { get; set; }
        public Int16 Seite_X { get; set; }
        public Int16 Spalte_Y { get; set; }
        public Int16 Zeile_Z { get; set; }
        public bool LPNR_aktiv { get; set; }
        public bool X_Y_Z_aktiv { get; set; }
        public bool SNR_aktiv { get; set; }
        public bool Starten { get; set; }
    }

    

    [PlcType("DB_ZK_Storage_BandG.Sonderfunktion")]
    public class DB_ZK_Storage_BandG_Sonderfunktion
    {
        public bool Reorganisieren { get; set; }
        public bool alles_loeschen { get; set; }
        public bool nur_inkons_reorg { get; set; }	//Nur inkonsistente Fächer Reorganisieren
    }

    

    [PlcType("DB_ZK_Storage_BandG.Batch")]
    public class DB_ZK_Storage_BandG_Batch
    {
        [ArrayBounds(1,10,0)]
        public Char[] LPNR { get; set; }

        [ArrayBounds(1,10,0)]
        public Char[] Sach_Nr { get; set; }

        [ArrayBounds(1,12,0)]
        public UDT_BatchData[] Platz { get; set; }
        public Int16 Anz { get; set; }	//max. Anzahl Batch
        public Int16 Pointer { get; set; }	//Zeiger Batch befüllen
        public Int16 PointerAusl { get; set; }	//Zeiger Batch Auslager
        public bool Complete { get; set; }	//Batch komplet zum Auslager
        public bool AnwAuslagern { get; set; }	//Batch Auslagern angewählt
        public bool SetData { get; set; }	//Daten Auslagern übergeben
        public bool Blocked { get; set; }	//Batch für Mindermenge blockiert
    }

    

    [PlcType("DB_ZK_Storage_BandG.IPS_C_auslagern")]
    public class DB_ZK_Storage_BandG_IPS_C_auslagern
    {
        [ArrayBounds(1,10,0)]
        public Char[] LPNR { get; set; }

        [ArrayBounds(1,10,0)]
        public Char[] Sach_NR { get; set; }
    }

    

    [PlcType("DB_ZK_Storage_BandG.Gasse.Seite_X.Spalte_Y")]
    public class DB_ZK_Storage_BandG_Gasse_Seite_X_Spalte_Y
    {
        [ArrayBounds(1,4,0)]
        public UDT_StorageData[] Zeile_Z { get; set; }
    }

    

    [PlcType("DB_ZK_Storage_BandG.Gasse.Seite_X")]
    public class DB_ZK_Storage_BandG_Gasse_Seite_X
    {
        [ArrayBounds(1,10,0)]
        public DB_ZK_Storage_BandG_Gasse_Seite_X_Spalte_Y[] Spalte_Y { get; set; }
    }

    

    [PlcType("DB_ZK_Storage_BandG.Gasse")]
    public class DB_ZK_Storage_BandG_Gasse
    {
        [ArrayBounds(1,2,0)]
        public DB_ZK_Storage_BandG_Gasse_Seite_X[] Seite_X { get; set; }	//rechte / linke Seite (links = 1, rechts = 2)
    }


    
    [PlcType("UDT_BatchData")]
    public class UDT_BatchData
    {

        [ArrayBounds(1,10,0)]
        public Char[] LPNR { get; set; }	//Lieferplannummer

        [ArrayBounds(1,10,0)]
        public Char[] Sach_NR { get; set; }	//Zylinderkopf Sachnummer

        [ArrayBounds(1,8,0)]
        public Char[] Ser_NR { get; set; }	//Zylinderkopf Seriennummer
        public DateTime Einlager_zeit { get; set; }
        [NotAccessible(true)]
        public Int16 Gasse { get; set; }
        [NotAccessible(true)]
        public Int16 Seite { get; set; }
        [NotAccessible(true)]
        public Int16 Spalte { get; set; }
        [NotAccessible(true)]
        public Int16 Zeile { get; set; }
        [NotAccessible(true)]
        public bool Ausgelagert { get; set; }

    }

    
    [PlcType("UDT_StorageData")]
    public class UDT_StorageData
    {

        [ArrayBounds(1,10,0)]
        public Char[] LPNR { get; set; }	//Lieferplannummer

        [ArrayBounds(1,10,0)]
        public Char[] Sach_NR { get; set; }	//Zylinderkopf Sachnummer

        [ArrayBounds(1,8,0)]
        public Char[] Ser_NR { get; set; }	//Zylinderkopf Seriennummer
        public DateTime Einlager_zeit { get; set; }
        public UDT_StorageData_Status Status { get; set; }

    }

    [Mapping("DB_ZK_Storage_BandG", "DB1241", 0)]
    public class DB_ZK_Storage_BandG
    {
        public DB_ZK_Storage_BandG_akt_Einlagern akt_Einlagern { get; set; }
        public DB_ZK_Storage_BandG_akt_Auslagern akt_Auslagern { get; set; }
        public DB_ZK_Storage_BandG_Sonderauslagern Sonderauslagern { get; set; }
        public DB_ZK_Storage_BandG_Sonderfunktion Sonderfunktion { get; set; }

        [ArrayBounds(0,20,0)]
        public DB_ZK_Storage_BandG_Batch[] Batch { get; set; }
        public Int16 Anz_freier_Plaetze { get; set; }
        public Int16 Anz_freier_Plaetze_SP1 { get; set; }
        public Int16 Anz_freier_Plaetze_SP2 { get; set; }
        public DB_ZK_Storage_BandG_IPS_C_auslagern IPS_C_auslagern { get; set; }

        [ArrayBounds(1,2,0)]
        public DB_ZK_Storage_BandG_Gasse[] Gasse { get; set; }

    }

}

