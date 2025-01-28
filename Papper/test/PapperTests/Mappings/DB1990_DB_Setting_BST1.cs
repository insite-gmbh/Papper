

using Papper.Attributes;
using System;

namespace Papper.Tests.Mappings
{
    
    

    [PlcType("DB_Setting_BST1.Typ_links.Bauteile")]
    public class DB_Setting_BST1_Typ_links_Bauteile
    {
        [ArrayBounds(1,4,0)]
        public Char[] KBEZ { get; set; }

        [ArrayBounds(1,10,0)]
        public Char[] SNR { get; set; }

        [ArrayBounds(1,2,0)]
        public Char[] BTCO { get; set; }
    }

    

    [PlcType("DB_Setting_BST1.Typ_links")]
    public class DB_Setting_BST1_Typ_links
    {
        [ArrayBounds(1,9,0)]
        public Char[] TypSachnummer { get; set; }

        [ArrayBounds(1,10,0)]
        public DB_Setting_BST1_Typ_links_Bauteile[] Bauteile { get; set; }
    }

    

    [PlcType("DB_Setting_BST1.Typ_rechts.Bauteile")]
    public class DB_Setting_BST1_Typ_rechts_Bauteile
    {
        [ArrayBounds(1,4,0)]
        public Char[] KBEZ { get; set; }

        [ArrayBounds(1,10,0)]
        public Char[] SNR { get; set; }

        [ArrayBounds(1,2,0)]
        public Char[] BTCO { get; set; }
    }

    

    [PlcType("DB_Setting_BST1.Typ_rechts")]
    public class DB_Setting_BST1_Typ_rechts
    {
        [ArrayBounds(1,9,0)]
        public Char[] TypSachnummer { get; set; }

        [ArrayBounds(1,10,0)]
        public DB_Setting_BST1_Typ_rechts_Bauteile[] Bauteile { get; set; }
    }

    

    [PlcType("DB_Setting_BST1.gescannte_Sachnummer.Bauteile")]
    public class DB_Setting_BST1_gescannte_Sachnummer_Bauteile
    {
        [ArrayBounds(1,4,0)]
        public Char[] KBEZ { get; set; }

        [ArrayBounds(1,10,0)]
        public Char[] SNR { get; set; }

        [ArrayBounds(1,2,0)]
        public Char[] BTCO { get; set; }
    }

    

    [PlcType("DB_Setting_BST1.gescannte_Sachnummer")]
    public class DB_Setting_BST1_gescannte_Sachnummer
    {
        [ArrayBounds(1,9,0)]
        public Char[] TypSachnummer { get; set; }

        [ArrayBounds(1,10,0)]
        public DB_Setting_BST1_gescannte_Sachnummer_Bauteile[] Bauteile { get; set; }
    }


    [Mapping("DB_Setting_BST1", "DB1990", 0)]
    public class DB_Setting_BST1
    {
        public DB_Setting_BST1_Typ_links Typ_links { get; set; }
        public DB_Setting_BST1_Typ_rechts Typ_rechts { get; set; }
        [AliasName("\"E79.4 von GeräteSS X0 FOM060 links zuweisen\"")]
        [SymbolicAccessName("\"E79.4 von GeräteSS X0 FOM060 links zuweisen\"")]
        public bool E79__4__von__GeräteSS__X0__FOM060__links__zuweisen { get; set; }
        [AliasName("\"E79.4 von GeräteSS X0 FOM060 rechts zuweisen\"")]
        [SymbolicAccessName("\"E79.4 von GeräteSS X0 FOM060 rechts zuweisen\"")]
        public bool E79__4__von__GeräteSS__X0__FOM060__rechts__zuweisen { get; set; }
        [AliasName("\"E79.5 von GeräteSS X0 FOM060 links zuweisen\"")]
        [SymbolicAccessName("\"E79.5 von GeräteSS X0 FOM060 links zuweisen\"")]
        public bool E79__5__von__GeräteSS__X0__FOM060__links__zuweisen { get; set; }
        [AliasName("\"E79.5 von GeräteSS X0 FOM060 rechts zuweisen\"")]
        [SymbolicAccessName("\"E79.5 von GeräteSS X0 FOM060 rechts zuweisen\"")]
        public bool E79__5__von__GeräteSS__X0__FOM060__rechts__zuweisen { get; set; }
        [AliasName("\"E79.4\"")]
        [SymbolicAccessName("\"E79.4\"")]
        public bool E79__4 { get; set; }
        [AliasName("\"E79.5\"")]
        [SymbolicAccessName("\"E79.5\"")]
        public bool E79__5 { get; set; }
        [AliasName("\"E79.5[]\"")]
        [SymbolicAccessName("\"E79.5[]\"")]
        public bool E99__5 { get; set; }
        public DB_Setting_BST1_gescannte_Sachnummer gescannte_Sachnummer { get; set; }

    }

}

