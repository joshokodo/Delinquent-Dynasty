using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NpcResponseActionDataDTO {
    public DynamicActionTypeDTO DynamicActionType;
    public int iterations;
    public List<GenericTargetDTO> targets;
    public bool anyCondition;
    public List<ConditionDTO> actionConditions;
}