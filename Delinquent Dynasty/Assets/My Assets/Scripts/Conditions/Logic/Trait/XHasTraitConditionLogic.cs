using Unity.Entities;
using UnityEngine;

public struct XHasTraitConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var intensity = 0;
        if (utils.TraitsLookup.TryGetBuffer(target, out DynamicBuffer<TraitElement> traits)){
            foreach (var trait in traits){
                if (trait.TraitType.Matches(conditionData.PrimaryEnumValue.GetTraitType())){
                    intensity = trait.Intensity;
                    break;

                }
            }
        }

        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, intensity,
            (int) conditionData.PrimaryNumberValue) == conditionData.ExpectedConditionValue;
    }
}