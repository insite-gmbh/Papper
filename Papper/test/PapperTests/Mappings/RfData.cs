using Papper.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Papper.Tests.Mappings
{
        public class UDT_WorkTime_Q_Time
        {
            [ArrayBounds(1, 8, 0)]
            public Char[] SDesc { get; set; }

            [ArrayBounds(1, 4, 0)]
            public Char[] Tim { get; set; }
        }



        public class UDT_Repa_Rep_Texts
        {
            [ArrayBounds(1, 80, 0)]
            public Char[] DeComp { get; set; }  //Defective Part

            [ArrayBounds(1, 80, 0)]
            public Char[] SYMP { get; set; }    //Symptom

            [ArrayBounds(1, 80, 0)]
            public Char[] TXT1 { get; set; }    //AdditionalInfo 1

            [ArrayBounds(1, 80, 0)]
            public Char[] PRTX { get; set; }    //WorkPieceCarrierRepairText
        }



        public class UDT_Repa_Error_MF
        {
            [ArrayBounds(1, 2, 0)]
            public Char[] code { get; set; }    //ErrorCode
        }



        public class UDT_Repa_Error
        {
            [ArrayBounds(1, 10, 0)]
            public Char[] FPNo { get; set; }    //ErrorPictureNumber

            [ArrayBounds(1, 80, 0)]
            public UDT_Repa_Error_MF[] MF { get; set; } //AssemblyPointFailure
        }



        public class UDT_Component_internal_Component
        {
            [ArrayBounds(1, 4, 0)]
            public Char[] SDesc { get; set; }   //ShortTitle

            [ArrayBounds(1, 14, 0)]
            public Char[] CompCo { get; set; }  //ComponentCode
        }



        public class UDT_Ma_Prog_MA_PROG
        {
            [ArrayBounds(1, 6, 0)]
            public Char[] STAT { get; set; }    //StationName

            [ArrayBounds(1, 2, 0)]
            public Char[] PROG { get; set; }    //MachineProgram
        }



        public class UDT_PartNo_PartNumber
        {
            [ArrayBounds(1, 4, 0)]
            public Char[] SDesc { get; set; }   //ShortTitle

            [ArrayBounds(1, 10, 0)]
            public Char[] PNo { get; set; } //PartNumber

            [ArrayBounds(1, 2, 0)]
            public Char[] CompCo { get; set; }  //ComponentCode
        }



        public class UDT_Engine_Dat_MOMO
        {
            [ArrayBounds(1, 4, 0)]
            public Char[] SDesc { get; set; }   //ShortTitle

            [ArrayBounds(1, 4, 0)]
            public Char[] DAT { get; set; } //Data
        }



        public class UDT_AwT_AwT_SnrSlot
        {
            [ArrayBounds(1, 10, 0)]
            public Char[] PNo { get; set; } //PartNumber
        }



        public class UDT_AwT_AwT
        {
            [ArrayBounds(1, 4, 0)]
            public Char[] SDesc { get; set; }   //ShortTitle

            [ArrayBounds(1, 7, 0)]
            public UDT_AwT_AwT_SnrSlot[] SnrSlot { get; set; }
        }



        public class UDT_WorkTime
        {

            [ArrayBounds(1, 50, 0)]
            public UDT_WorkTime_Q_Time[] Q_Time { get; set; }

        }


        public class UDT_Repa
        {
            public UDT_Repa_Rep_Texts Rep_Texts { get; set; }   //Manual Rep-Info-Texts

            [ArrayBounds(1, 5, 0)]
            public UDT_Repa_Error[] Error { get; set; }

        }


        public class UDT_StandBy_Text
        {

            [ArrayBounds(1, 80, 0)]
            public Char[] TXT1 { get; set; }    //Stand-By Info Text 1

            [ArrayBounds(1, 80, 0)]

        public Char[] TXT2 { get; set; }    //Stand-By Info Text 2


        [ArrayBounds(1, 80, 0)]
            public Char[] TXT3 { get; set; }    //Stand-By Info Text 3

            [ArrayBounds(1, 80, 0)]
            public Char[] TXT4 { get; set; }    //Stand-By Info Text 6

            [ArrayBounds(1, 80, 0)]
            public Char[] TXT5 { get; set; }    //Stand-By Info Text 5

        }


        public class UDT_Component_internal
        {

            [ArrayBounds(1, 29, 0)]
            public UDT_Component_internal_Component[] Component { get; set; }   //ComponentTitle

        }


        public class UDT_Ma_Prog
        {

            [ArrayBounds(1, 75, 0)]
            public UDT_Ma_Prog_MA_PROG[] MA_PROG { get; set; }

        }


        public class UDT_PartNo
        {

            [ArrayBounds(1, 131, 0)]
            public UDT_PartNo_PartNumber[] PartNumber { get; set; }

            [ArrayBounds(2096, 2099, 0)]
            public Char[] Res_2096_2099 { get; set; }

        }


        public class UDT_Engine_Dat
        {

            [ArrayBounds(1, 4, 0)]
            public Char[] EngineNo_SDesc { get; set; }  //title Enginenumber

            [ArrayBounds(1, 14, 0)]
            public Char[] EngineNo { get; set; }    //Engine-Number

            [ArrayBounds(1, 4, 0)]
            public Char[] Prod_SDesc { get; set; }  //title EnginePartNumber

            [ArrayBounds(1, 14, 0)]
            public Char[] PROD { get; set; }    //EnginePartNumber

            [ArrayBounds(1, 4, 0)]
            public Char[] Lpnr_SDesc { get; set; }  //Title DeliveryPlanNumber

            [ArrayBounds(1, 14, 0)]
            public Char[] Lpnr { get; set; }    //DeliveryPlanNumber

            [ArrayBounds(1, 4, 0)]
            public Char[] MOAR_SDesc { get; set; }  //title EngineVariant

            [ArrayBounds(1, 14, 0)]
            public Char[] MOAR { get; set; }    //EngineVariant

            [ArrayBounds(1, 4, 0)]
            public Char[] APOI_SDesc { get; set; }  //title APO-Id

            [ArrayBounds(1, 14, 0)]
            public Char[] Apoi { get; set; }    //APO-Id

            [ArrayBounds(1, 4, 0)]
            public Char[] SEQI_BEZ { get; set; }    //title Sequence-Id

            [ArrayBounds(1, 14, 0)]
            public Char[] SEQI { get; set; }    //Sequence-Id

            [ArrayBounds(1, 4, 0)]
            public Char[] SMAT_SDesc { get; set; }  //title SMAT

            [ArrayBounds(1, 14, 0)]
            public Char[] SMAT { get; set; }    //Time Engine from Conveyor

            [ArrayBounds(1, 4, 0)]
            public Char[] TYP_SDesc { get; set; }   //title Type

            [ArrayBounds(1, 14, 0)]
            public Char[] typ { get; set; } //EngineType

            [ArrayBounds(1, 4, 0)]
            public Char[] ABE_SDesc { get; set; }   //title ABE

            [ArrayBounds(1, 14, 0)]
            public Char[] ABEK { get; set; }    //ABE Id

            [ArrayBounds(1, 4, 0)]
            public Char[] SNMO_SDesc { get; set; }  //title SNMO

            [ArrayBounds(1, 14, 0)]
            public Char[] SNMO { get; set; }    //CustomerPartNumber

            [ArrayBounds(1, 4, 0)]
            public Char[] ABLO_SDesc { get; set; }  //title UnloadingDestination

            [ArrayBounds(1, 14, 0)]
            public Char[] ABLO { get; set; }    //UnloadingPlace

            [ArrayBounds(1, 4, 0)]
            public Char[] BEMO_SDesc { get; set; }  //Title CalculatingMode

            [ArrayBounds(1, 14, 0)]
            public Char[] BEMO { get; set; }    //CalculatingMode

            [ArrayBounds(216, 399, 0)]
            public Char[] RES_216bis399 { get; set; }   //Reserved for Engine-Data

            [ArrayBounds(1, 37, 0)]
            public UDT_Engine_Dat_MOMO[] MOMO { get; set; } //MOMO-Field

            [ArrayBounds(1, 4, 0)]
            public Char[] Reserve { get; set; }

        }


        public class UDT_Dest_Dat
        {

            [ArrayBounds(1, 4, 0)]
            public Char[] WPC_Number { get; set; }  //WorkPieceCarrierNumber
            public Char WPC_Status { get; set; }    //WPC Status
            public Char WPC_Rep { get; set; }   //WPC Repair

            [ArrayBounds(1, 2, 0)]
            public Char[] Circulation_Count { get; set; }   //CirculationCounter WPC

            [ArrayBounds(1, 8, 0)]
            public Char[] Source_Direct { get; set; }   //source direct

            [ArrayBounds(1, 8, 0)]
            public Char[] Dest_Direct { get; set; } //destination direct

            [ArrayBounds(1, 8, 0)]
            public Char[] Source_Rep_Info { get; set; } //source Rep-Info

            [ArrayBounds(1, 8, 0)]
            public Char[] Dest_Ind_processed { get; set; }  //destination Rep indirect: Process

            [ArrayBounds(1, 8, 0)]
            public Char[] Dest_Ind_Rep { get; set; }    //destination Rep indirect: RepairStation

            [ArrayBounds(1, 8, 0)]
            public Char[] Source_STBY_Info { get; set; }    //source Stand-By Info

            [ArrayBounds(1, 8, 0)]
            public Char[] Dest_Stby_Info { get; set; }  //Destination Stand-By Info

            [ArrayBounds(1, 8, 0)]
            public Char[] FarDestination { get; set; }  //FarDestination

            [ArrayBounds(1, 8, 0)]
            public Char[] FarDestination_res1 { get; set; } //FarDestination

            [ArrayBounds(1, 8, 0)]
            public Char[] FarDestination_res2 { get; set; } //FarDestination
            public Char Engine_NOK { get; set; }    //Engine is nok
            public Char LeakTest_OK { get; set; }   //LeakTest iq
            public Char OilFilledStatus { get; set; }   //OilFilledStatus
            public Char Mech_ColdTest_OK { get; set; }  //Mechanic / ColdTest iq

            [ArrayBounds(1, 4, 0)]
            public Char[] TestCounter_LeakTest { get; set; }    //testruncounter LeakTest

            [ArrayBounds(1, 4, 0)]
            public Char[] TestCounter_Mec_ColdTest { get; set; }    //testruncounter Mechanic / ColdTest

            [ArrayBounds(1, 4, 0)]
            public Char[] iq_TestCounter_LeakTest { get; set; } //OK-testruncounter LeakTest

            [ArrayBounds(1, 4, 0)]
            public Char[] iq_TestCnt_Mec_ColdTest { get; set; } //OK-testruncounter Mechanic / ColdTest
            public Char CS_Request { get; set; }    //CS-Request is happen
            public Char Res109 { get; set; }

            [ArrayBounds(110, 177, 0)]
            public Char[] RES_110BIS177 { get; set; }

        }


        public class UDT_AwT
        {

            [ArrayBounds(1, 22, 0)]
            public UDT_AwT_AwT[] AwT { get; set; }  //ComponentTitle

            [StringLength(70)]
            public string Reserve { get; set; }

        }


        public class UDT_RF_Dat
        {
            public UDT_Dest_Dat Course { get; set; }
            public UDT_Component_internal Comp_internal { get; set; }
            public UDT_Engine_Dat EngineData { get; set; }
            public UDT_PartNo Partno { get; set; }
            public UDT_Ma_Prog Masch_Prog { get; set; }
            public UDT_AwT SelectTab { get; set; }
            public UDT_WorkTime WorkTime { get; set; }
            public UDT_StandBy_Text Text_StandBy { get; set; }
            public UDT_Repa Repa { get; set; }

        }

        [Mapping("DB_DATA_RF_BST1_ST", "DB1114", 0)]
        [Mapping("DB_DATA_RF_BST2_ST", "DB2114", 0)]
        [Mapping("DB_DATA_RF_BST3_ST", "DB3114", 0)]
        [Mapping("DB_DATA_RF_BST4_ST", "DB4114", 0)]
        [Mapping("DB_DATA_RF_BST5_ST", "DB5114", 0)]
        [Mapping("DB_DATA_RF_BST6_ST", "DB6114", 0)]
        [Mapping("DB_DATA_RF_BST1_PST", "DB1112", 0)]
        [Mapping("DB_DATA_RF_BST2_PST", "DB2112", 0)]
        [Mapping("DB_DATA_RF_BST3_PST", "DB3112", 0)]
        [Mapping("DB_DATA_RF_BST4_PST", "DB4112", 0)]
        [Mapping("DB_DATA_RF_BST5_PST", "DB5112", 0)]
        [Mapping("DB_DATA_RF_BST6_PST", "DB6112", 0)]
        public class RfData
        {
            public UDT_RF_Dat DATA { get; set; }
        }


}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.