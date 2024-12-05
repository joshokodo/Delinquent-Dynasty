using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct XIsDistanceFromYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var room = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        if (utils.LocalTransformLookup.TryGetComponent(target, out LocalTransform targetTransform)
            && utils.LocalTransformLookup.TryGetComponent(room, out LocalTransform roomTransform)){
            var distance = Vector3.Distance(targetTransform.Position, roomTransform.Position);
            var withinRange = NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, (int) distance,
                (int) conditionData.PrimaryNumberValue);
            return withinRange == conditionData.ExpectedConditionValue;
        }

        return !conditionData.ExpectedConditionValue;
    }
}