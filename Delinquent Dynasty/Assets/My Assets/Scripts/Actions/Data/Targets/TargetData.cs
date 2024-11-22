using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct TargetData {
    public TargetType TargetType;
    public bool FindGenericTarget;
    public bool AnyCondition;
    public Entity TargetEntity;
    public float3 TargetWorldPoint;
    public FixedString32Bytes TargetString;
    public int CountValue;
    public int SellValue;
    public DynamicGameEnum EnumValue;
}