using System;
using Unity.Collections;
using Unity.Entities;

public struct ActiveEffectComponent : IComponentData {
    public Entity SourceEntity;
    public YesNoChoiceType IsValid;
    public TargetsGroup TargetsData;
    public bool AnyCondition;
    public FixedList512Bytes<ActiveEffectData> Effects;
    public FixedList4096Bytes<ConditionData> Conditions;
}