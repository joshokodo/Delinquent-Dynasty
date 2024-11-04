using System;
using System.Collections.Generic;

[Serializable]
public class PassiveEffectsAndConditionsDTO {
    public List<PassiveEffectDTO> effects;
    public bool anyCondition;
    public List<ConditionDTO> conditions;
}