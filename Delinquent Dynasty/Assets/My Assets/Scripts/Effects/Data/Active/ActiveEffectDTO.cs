using System;
using Unity.Collections;

[Serializable]
public class ActiveEffectDTO {
    public ActiveEffectType activeEffectType;
    public TargetType effectPrimaryTarget;
    public TargetType effectSecondaryTarget;
    public TargetType effectTertiaryTarget;
    public bool displayToWorld;
    public bool originLearnsKnowledge;

    public DynamicGameEnumDTO primaryEnumValue;
    public DynamicGameEnumDTO secondaryEnumValue;
    public DynamicGameEnumDTO tertiaryEnumValue;

    public NumberScaleType primaryNumberScale;
    public DynamicGameEnumDTO primaryEnumScale;
    public int minIntValue;
    public int maxIntValue;

    public ActiveEffectData ToData(){
        return new ActiveEffectData(){
            ActiveEffectType = activeEffectType,
            EffectPrimaryTarget = effectPrimaryTarget,
            EffectSecondaryTarget = effectSecondaryTarget,
            EffectTertiaryTarget = effectTertiaryTarget,
            PrimaryNumberScale = primaryNumberScale,
            PrimaryNumberScaleEnum = primaryEnumScale.ToData(),
            DisplayToWorld = displayToWorld,
            SourceLearnsKnowledge = originLearnsKnowledge,
            PrimaryNumberRangeValue = new IntRangeStatData(){
                Min = minIntValue,
                Max = maxIntValue
            },
            PrimaryEnumValue = primaryEnumValue.ToData(),
            SecondaryEnumValue = secondaryEnumValue.ToData(),
            TertiaryEnumValue = tertiaryEnumValue.ToData(),
        };
    }
}