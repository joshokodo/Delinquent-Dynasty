using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct XIsSkillLevelConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var value = 0;
        foreach (var skill in utils.SkillsLookup[target]){
            if (skill.SkillType == conditionData.PrimaryEnumValue.SkillType){
                value = skill.CurrentLevel;
            }
        }

        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, value,
            (int) conditionData.PrimaryNumberValue) == conditionData.ExpectedConditionValue;
    }
}