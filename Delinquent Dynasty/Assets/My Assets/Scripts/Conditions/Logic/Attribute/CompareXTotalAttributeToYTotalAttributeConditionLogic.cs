using Unity.Entities;

public struct CompareXTotalAttributeToYTotalAttributeConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var y = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        if (utils.CharacterBioLookup.HasComponent(x) && utils.CharacterBioLookup.HasComponent(y)){
            var xPassives = utils.CreatePassiveUtils(x);
            var yPassives = utils.CreatePassiveUtils(y);
            var xTotal = xPassives.GetNaturalAndBonusAttributeTotal(conditionData.PrimaryEnumValue.AttributeType);
            var yTotal = yPassives.GetNaturalAndBonusAttributeTotal(conditionData.PrimaryEnumValue.AttributeType);
            return conditionData.ExpectedConditionValue ==
                   NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, xTotal, yTotal);
        }

        return !conditionData.ExpectedConditionValue;
    }
}