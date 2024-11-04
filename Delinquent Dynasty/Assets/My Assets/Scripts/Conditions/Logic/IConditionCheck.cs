using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData);
}