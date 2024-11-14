public struct XIsAttributeTotalLevelConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var val = 0;
        if (utils.TryCreatePassiveUtils(target, out PassiveEffectsUtils passives)){
            val = passives.GetNaturalAndBonusAttributeTotal(conditionData.PrimaryEnumValue.AttributeType);
        }

        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, val, (int) conditionData.PrimaryNumberValue) ==
               conditionData.ExpectedConditionValue;
    }
}