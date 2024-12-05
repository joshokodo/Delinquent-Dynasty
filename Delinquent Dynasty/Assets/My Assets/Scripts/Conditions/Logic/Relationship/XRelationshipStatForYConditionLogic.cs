using Unity.Entities;
using UnityEngine;

public struct XRelationshipStatForYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var y = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);

        var stat = 0;
        if (x == Entity.Null || !utils.CharacterBioLookup.HasComponent(x)
                             || y == Entity.Null || !utils.CharacterBioLookup.HasComponent(y)){
            return conditionData.ExpectedConditionValue == false;
        }

        var relationships = utils.RelationshipLookup[x];
        foreach (var relationshipElement in relationships){
            if (relationshipElement.Character == y){
                stat =
                    relationshipElement.GetStat(conditionData.PrimaryEnumValue.RelationshipStatType);
                break;
            }
        }

        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, stat,
            (int) conditionData.PrimaryNumberValue) == conditionData.ExpectedConditionValue;
    }
}