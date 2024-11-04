using Unity.Collections;
using Unity.Entities;

public struct PassiveEffectData {
    public PassiveEffectType PassiveEffectType;
    public PassiveEffectSubjectType PassiveEffectSubject;
    public PassiveEffectTriggerType PassiveEffectTrigger;

    public bool ApplyToActionTarget;
    public double PassiveEffectTriggerSecondsRate;

    public IntRangeStatData PrimaryNumberRangeValue;
    public DynamicActionType PrimaryActionType;
    public DynamicGameEnum PrimaryEnumValue;
    public DynamicGameEnum SecondaryEnumValue;
    public NumberScaleType BonusNumericType;
    public DynamicGameEnum BonusEnumValue;
    public bool DisplayTriggeredEffect;
    public bool SourcesLearnEffect;
}