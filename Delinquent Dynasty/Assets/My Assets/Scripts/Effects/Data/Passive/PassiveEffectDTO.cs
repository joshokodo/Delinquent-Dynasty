using System;
using UnityEngine.Serialization;

[Serializable]
public class PassiveEffectDTO {
    public PassiveEffectTriggerType passiveEffectTriggerType;
    public PassiveEffectSubjectType passiveEffectSubjectType;
    public PassiveEffectType passiveEffectType;
    public NumberScaleType bonusNumericType;
    public DynamicGameEnumDTO bonusEnumValue;
    public DynamicGameEnumDTO primaryEnumValue;
    public DynamicGameEnumDTO secondaryEnumValue;

    [FormerlySerializedAs("primaryDynamicActionType")]
    public DynamicActionTypeDTO primaryActionType;

    public double effectsOverTimeSecondsRate;
    public bool applyToActionTargetCharacter;
    public bool displayTriggerEffect;
    public bool sourcesLearnEffect;

    public int minIntValue;
    public int maxIntValue;

    public PassiveEffectData ToData(){
        return new PassiveEffectData(){
            PassiveEffectType = passiveEffectType,
            PassiveEffectSubject = passiveEffectSubjectType,
            PassiveEffectTrigger = passiveEffectTriggerType,
            PassiveEffectTriggerSecondsRate = effectsOverTimeSecondsRate,
            ApplyToActionTarget = applyToActionTargetCharacter,
            DisplayTriggeredEffect = displayTriggerEffect,
            SourcesLearnEffect = sourcesLearnEffect,
            PrimaryNumberRangeValue = new IntRangeStatData(){
                Min = minIntValue,
                Max = maxIntValue,
            },
            BonusNumericType = bonusNumericType,
            BonusEnumValue = bonusEnumValue.ToData(),
            PrimaryEnumValue = primaryEnumValue.ToData(),
            SecondaryEnumValue = secondaryEnumValue.ToData(),
            PrimaryActionType = primaryActionType.ToData()
        };
    }
}