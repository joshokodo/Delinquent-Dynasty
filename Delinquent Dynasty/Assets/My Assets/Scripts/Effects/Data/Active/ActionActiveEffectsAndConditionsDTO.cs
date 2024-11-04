using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct ActionActiveEffectsAndConditionsDTO {
    public int skillLevelRequirement;
    public NumericComparisonSign skillNumericComparison;
    public List<ActiveEffectDTO> effects;
    public bool anyCondition;
    public List<ConditionDTO> conditions;
}