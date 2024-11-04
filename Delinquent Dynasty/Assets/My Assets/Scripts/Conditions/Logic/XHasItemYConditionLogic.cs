using Unity.Entities;
using UnityEngine;

// todo: IMPORTANT! added null safety to all conditions
public struct XHasItemYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var targetItem = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        var isOnPerson = false;
        foreach (var itemElement in utils.ItemsLookup[target]){
            if (itemElement.ItemEntity == targetItem){
                isOnPerson = true;
                break;
            }
        }

        return isOnPerson == conditionData.ExpectedConditionValue;
    }
}