using System;
using Unity.Collections;
using Unity.Entities;

public struct PassiveEffectComponent : IComponentData {
    public FixedString64Bytes PassiveTitle;

    public Entity PrimaryTarget;
    public TargetType PrimaryTargetType;
    public Entity SecondaryTarget;
    public TargetType SecondaryTargetType;

    public Entity SourceEntity;
    public Guid PassiveAssetId;
    public Guid OriginActionId;
    public DynamicGameEnum SourcePrimaryEnum;
    public DynamicGameEnum SourceSecondaryEnum;

    public PassiveEffectData Data;

    public bool AnyCondition;
    public FixedList4096Bytes<ConditionData> Conditions;

    public PassiveEffectSourceType SourceType;
    public bool IsValid;
    public bool SkipChecks;
    public bool SkipNextFrame;
    public double EffectsOverTimeStartTime;

    public int NextIntValue;

    public void SetNextIntValue(RefRW<RandomComponent> randomComponent){
        NextIntValue = Data.PrimaryNumberRangeValue.GetNextInt(randomComponent);
    }
}