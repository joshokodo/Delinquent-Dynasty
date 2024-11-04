using System;
using System.Collections.Generic;

[Serializable]
public class ActiveEffectsAndConditionsDTO {
    public List<ActiveEffectDTO> effects;
    public bool anyCondition;
    public List<ConditionDTO> conditions;
}