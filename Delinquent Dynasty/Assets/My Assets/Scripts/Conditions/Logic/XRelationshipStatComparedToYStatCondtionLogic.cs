using Unity.Entities;
using UnityEngine;

public struct XRelationshipStatComparedToYStatCondtionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var y = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);

        if (x == Entity.Null || !utils.CharacterBioLookup.HasComponent(x)
                             || y == Entity.Null || !utils.CharacterBioLookup.HasComponent(y)){
            return !conditionData.ExpectedConditionValue;
        }

        var xRelationships = utils.RelationshipLookup[x];
        var yRelationships = utils.RelationshipLookup[y];
        var xOpinion = 0;
        var yOpinion = 0;
        foreach (var relationshipElement in xRelationships){
            if (relationshipElement.Character == y){
                xOpinion =
                    relationshipElement.GetStat(conditionData.PrimaryEnumValue.RelationshipStatType);
                break;
            }
        }

        foreach (var relationshipElement in yRelationships){
            if (relationshipElement.Character == x){
                yOpinion =
                    relationshipElement.GetStat(conditionData.SecondaryEnumValue.RelationshipStatType);
                break;
            }
        }

        Debug.Log("is " + xOpinion + " " + conditionData.NumericComparisonSign + " " + yOpinion + " " + NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, xOpinion, yOpinion));
        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, xOpinion, yOpinion)
               == conditionData.ExpectedConditionValue;
    }
}