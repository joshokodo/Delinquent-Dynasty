using System;
using System.Collections.Generic;
using Unity.Collections;

[Serializable]
public class ActionPassiveEffectsAndConditionsDTO {
    public string passiveTitle;
    public int skillLevelRequirement;
    public NumericComparisonSign skillLevelComparisonSign;
    public List<PassiveEffectDTO> effects;
    public bool anyCondition;
    public List<ConditionDTO> conditions;
}