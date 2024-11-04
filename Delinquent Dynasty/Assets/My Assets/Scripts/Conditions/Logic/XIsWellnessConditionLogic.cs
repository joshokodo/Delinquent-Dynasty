using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct XIsWellnessConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        //TODO: important add passive utils to get bonus wellness ect. do for all these conditions
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var wellnessUtils = new WellnessUtils(){
            Wellness = utils.WellnessLookup[target]
        };
        var wellness =
            wellnessUtils.GetCurrentValue(conditionData.PrimaryEnumValue.WellnessType);
        var value = (int) conditionData.PrimaryNumberValue;
        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, wellness,
            value) == conditionData.ExpectedConditionValue;
    }
}