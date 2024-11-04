using System;
using Unity.Collections;
using Unity.Entities;

public struct ActiveEffectAssetData {
    public Guid ParentId;
    public bool AnyCondition;
    public FixedList512Bytes<ActiveEffectData> Effects;
    public FixedList4096Bytes<ConditionData> Conditions;
}