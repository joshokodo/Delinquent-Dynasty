using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

public struct ItemXHasUnreadTextCountForYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var count = 0;
        var target = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        var targetItem = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);

        if (utils.CharacterKnowledgeLookup.TryGetBuffer(targetItem,
                out DynamicBuffer<CharacterKnowledgeElement> phoneKnowledgeElements)){
            foreach (var phoneKnowledgeElement in phoneKnowledgeElements){
                if (utils.TextMessageKnowledgeLookup.TryGetComponent(phoneKnowledgeElement.KnowledgeEntity,
                        out TextMessageKnowledgeComponent text)){
                    if (text.BaseData.OriginPhone == targetItem &&
                        utils.ItemCellPhoneLookup[text.BaseData.TargetPhone].Owner ==
                        target && !text.BaseData.IsRead){
                        count++;
                    }
                }
            }
        }

        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, count,
            (int) conditionData.PrimaryNumberValue) == conditionData.ExpectedConditionValue;
    }
}