

using Papper.Attributes;
#pragma warning disable CA1707 // Identifiers should not contain underscores
#pragma warning disable CA1819 // Properties should not return arrays
namespace Papper.Tests.Mappings
{



    public class UDT_AI_Texte_AI_Suchtext_MOMO
    {
        [ArrayBounds(1, 4, 0)]
        public char[] Bez { get; set; }	//MOMO -> Bezeichnung (wird am Datenträger gesucht)

        [ArrayBounds(1, 4, 0)]

        public char[] Daten { get; set; }   //MOMO -> Daten wird mit Datenträger verglichen

    }



    public class UDT_AI_Texte_AI_Suchtext
    {
        [ArrayBounds(1, 10, 0)]
        public char[] PROD { get; set; }	//Produktnummer (Motorsachnummer) -> fixe Position am Datenträger

        [ArrayBounds(1, 4, 0)]
        public char[] MOAR { get; set; }	//Motorart  -> fixe Position am Datenträger
        public UDT_AI_Texte_AI_Suchtext_MOMO MOMO { get; set; }
    }



    public class UDT_AI_Texte_AI_Anzeige
    {
        public short Pos { get; set; }	//Position im Bild (von oben nach unten: 1,2,3) 0 = beliebige Position

        [ArrayBounds(1, 4, 0)]
        public char[] Snr_KzBez { get; set; }	//Sachnummern-Kurzbezeichnung (wird am Datenträger gesucht)

        [ArrayBounds(1, 80, 0)]
        public char[] Text { get; set; }	//Anzuzeigender Text -> Arbeitsinhalt
    }



    public class UDT_AI_Texte_AI
    {
        public UDT_AI_Texte_AI_Suchtext Suchtext { get; set; }
        public UDT_AI_Texte_AI_Anzeige Anzeige { get; set; }
    }



    public class UDT_AI_Texte
    {

        [ArrayBounds(1, 50, 0)]
        public UDT_AI_Texte_AI[] AI { get; set; }

    }

    [Mapping("DB_AI_Texte_BST1", "DB1012", 0)]
    public class DB_AI_Texte_BST1
    {
        public UDT_AI_Texte BST { get; set; }

    }

}
#pragma warning restore CA1707 // Identifiers should not contain underscores
#pragma warning restore CA1819 // Properties should not return arrays
