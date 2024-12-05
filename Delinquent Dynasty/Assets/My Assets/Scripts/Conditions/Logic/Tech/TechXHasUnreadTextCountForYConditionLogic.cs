using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

public struct TechXHasUnreadTextCountForYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var count = 0;
        var targetItem = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var target = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);

        if (utils.TextMessageLookup.TryGetBuffer(targetItem,
                out DynamicBuffer<TextMessageElement> texts)){
            foreach (var text in texts){
                if (!text.HasRead && text.OriginPhone != targetItem && utils.ItemCellPhoneLookup[text.OriginPhone].Owner == target){
                    count++;
                }
            }
        }

        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, count,
            (int) conditionData.PrimaryNumberValue) == conditionData.ExpectedConditionValue;
    }
}