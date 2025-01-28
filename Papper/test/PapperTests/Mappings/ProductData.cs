using Papper.Attributes;

namespace Papper.Tests.Mappings
{
    public class ValveHousing
    {
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    public class ToleranceShort
    {
        public short Min { get; set; }
        public short Max { get; set; }
    }

    public class ToleranceFloat
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }

    public class ResultBool
    {
        public bool Val { get; set; }
    }

    public class ResultShort
    {
        public short Val { get; set; }
    }

    public class ResultFloat
    {
        public float Val { get; set; }
    }

    public class SetPointShort
    {
        public short Set { get; set; }
    }

    public class SetPointFloat
    {
        public float Set { get; set; }
    }

    public class SetPointWithTolShort
    {
        public SetPointShort Set { get; set; } = new();
        public ToleranceShort Tol { get; set; } = new();
    }

    public class SetPointWithTolFloat
    {
        public SetPointFloat Set { get; set; } = new();
        public ToleranceFloat Tol { get; set; } = new();
    }

    public class DeburringData
    {
        public SetPointWithTolFloat HighPressure { get; set; } = new();
        public ToleranceFloat MaxTemperature { get; set; } = new();
        public ToleranceFloat Conductivity { get; set; } = new();
    }

    public class DeburringResults
    {
        public ResultFloat MaxTemperature { get; set; } = new();
        public ResultFloat HighPressure { get; set; } = new();
        public ResultFloat Conductivity { get; set; } = new();
    }

    public class WashingData
    {
        public SetPointWithTolFloat Temperature { get; set; } = new();
        public SetPointWithTolFloat Pressure { get; set; } = new();
        public ToleranceFloat Conductivity { get; set; } = new();
        public ToleranceShort Duration { get; set; } = new();
        public SetPointShort CyclesUpperNozzleBlock { get; set; } = new();
        public SetPointShort CyclesSideNozzleBlock { get; set; } = new();
        public SetPointShort TimeFixedBlock { get; set; } = new();
    }

    public class WashingResults
    {
        public float Temperature { get; set; } = new();
        public ResultFloat Pressure { get; set; } = new();
        public ResultFloat Conductivity { get; set; } = new();
        public ResultShort Duration { get; set; } = new();
        public ResultShort CyclesUpperNozzleBlock { get; set; } = new();
        public ResultShort CyclesSideNozzleBlock { get; set; } = new();
        public ResultShort TimeFixedBlock { get; set; } = new();
    }

    public class HotAirDryingData
    {
        public SetPointWithTolFloat Temperature { get; set; } = new();
        public ToleranceShort Duration { get; set; } = new();
        public SetPointShort CyclesUpperNozzleBlock { get; set; } = new();
        public SetPointShort CyclesSideNozzleBlock { get; set; } = new();
    }

    public class HotAirDryingResults
    {
        public ResultFloat Temperature { get; set; } = new();
        public ResultShort Duration { get; set; } = new();
        public ResultShort CyclesUpperNozzleBlock { get; set; } = new();
        public ResultShort CyclesSideNozzleBlock { get; set; } = new();
    }

    public class VacuumDryingData
    {
        public SetPointFloat ProcessPressure { get; set; } = new();
        public SetPointShort DryingTime { get; set; } = new();
        public ToleranceShort Duration { get; set; } = new();
    }

    public class VacuumDryingResults
    {
        public ResultFloat ProcessPressure { get; set; } = new();
        public ResultShort Duration { get; set; } = new();
        public ResultShort DryingTime { get; set; } = new();
    }

    public class GeneralResults
    {
        public ResultShort Position { get; set; } = new();
        public ResultShort IdDeburringMachine { get; set; } = new();
        public ResultShort NumberOfDeburrings { get; set; } = new();
        public ResultShort ProcessingTime { get; set; } = new();
        public ResultShort ProcessingState { get; set; } = new();
    }

    public enum EDeburringResultIdIndex
    {
        Deburring_MaxTemperature_0,
        Deburring_MaxTemperature_1,
        Deburring_MaxTemperature_2,
        Deburring_MaxTemperature_3,
        Deburring_HighPressure_0,
        Deburring_HighPressure_1,
        Deburring_HighPressure_2,
        Deburring_HighPressure_3,
        Deburring_Conductivity_0,
        Deburring_Conductivity_1,
        Deburring_Conductivity_2,
        Deburring_Conductivity_3,
        LastResultIndexPlusOne
    }

    public enum EUnloadingResultIdIndex
    {
        Washing_Temperature = EDeburringResultIdIndex.LastResultIndexPlusOne,
        Washing_Pressure,
        Washing_Conductivity,
        Washing_Duration,
        Washing_CyclesUpperNozzleBlock,
        Washing_CyclesSideNozzleBlock,
        Washing_TimeFixedBlock,
        HotAirDrying_Temperature,
        HotAirDrying_Duration,
        HotAirDrying_CyclesUpperNozzleBlock,
        HotAirDrying_CyclesSideNozzleBlock,
        VacuumDrying_ProcessPressure_0,
        VacuumDrying_ProcessPressure_1,
        VacuumDrying_ProcessPressure_2,
        VacuumDrying_Duration_0,
        VacuumDrying_Duration_1,
        VacuumDrying_Duration_2,
        VacuumDrying_DryingTime_0,
        VacuumDrying_DryingTime_1,
        VacuumDrying_DryingTime_2,
        LastResultIndexPlusOne
    }

    public class ProductData
    {
        [ArrayBounds(1, 12, 0)]
        public char[] PartNumber { get; set; } = new char[12];
        public uint PartSetupDate { get; set; }
        public ValveHousing ValveHousing { get; set; } = new();
        public SetPointShort MaxNumDeburringCycles { get; set; } = new();
        [ArrayBounds(1, 4, 0)]
        public DeburringData[] Deburring { get; set; } = { new DeburringData(), new DeburringData(), new DeburringData(), new DeburringData() };
        public WashingData Washing { get; set; } = new();
        public HotAirDryingData HotAirDrying { get; set; } = new();
        public VacuumDryingData VacuumDrying { get; set; } = new();
        public SetPointShort AutoVisualInspection { get; set; } = new();
        public GeneralResults GeneralResults { get; set; } = new();
        public DeburringResults DeburringResults { get; set; } = new();
        public WashingResults WashingResults { get; set; } = new();
        public HotAirDryingResults HotAirDryingResults { get; set; } = new();
        [ArrayBounds(1, 3, 0)]
        public VacuumDryingResults[] VacuumDryingResults { get; set; } = { new VacuumDryingResults(), new VacuumDryingResults(), new VacuumDryingResults() };
        public bool DeburringOK { get; set; }
        public bool DeburringMaxTemperatureOK { get; set; }
        public bool DeburringHighPressureOK { get; set; }
        public bool DeburringConductivityOK { get; set; }
        public bool WashingTemperatureOK { get; set; }
        public bool WashingPressureOK { get; set; }
        public bool WashingConductivityOK { get; set; }
        public bool WashingDurationOK { get; set; }
        public bool HotAirDryingTemperatureOK { get; set; }
        public bool HotAirDryingDurationOK { get; set; }
        public bool VacuumDrying1DurationOK { get; set; }
        public bool VacuumDrying2DurationOK { get; set; }
        public bool VacuumDrying3DurationOK { get; set; }
        [ArrayBounds(1, 14, 0)]
        public char[] ReleaseDate { get; set; } = new char[14];
        [ArrayBounds(1, 32, 0)]
        public ulong[] ResultIds { get; set; } = new ulong[32];
        [ArrayBounds(1, 32, 0)]
        public ulong[] FeatureIds { get; set; } = new ulong[32];
        [ArrayBounds(1, 24, 0)]
        public byte[] Reserved { get; set; } = new byte[24];
    }
}

