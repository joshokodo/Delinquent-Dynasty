using System;
using Unity.Collections;

public struct PassiveEffectAssetData {
    public Guid ParentId;
    public Guid Id;
    public bool AnyCondition;
    public FixedList4096Bytes<PassiveEffectData> Effects;
    public FixedList4096Bytes<ConditionData> Conditions;
}