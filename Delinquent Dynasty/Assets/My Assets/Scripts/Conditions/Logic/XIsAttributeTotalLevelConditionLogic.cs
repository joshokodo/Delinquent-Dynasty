public struct XIsAttributeTotalLevelConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        if (utils.TryCreatePassiveUtils(target, out PassiveEffectsUtils passives)){
            var currentLevel = passives.GetNaturalAndBonusAttributeTotal(conditionData.PrimaryEnumValue.AttributeType);
            var value = (int) conditionData.PrimaryNumberValue;
            return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, currentLevel, value) ==
                   conditionData.ExpectedConditionValue;
        }

        return !conditionData.ExpectedConditionValue;
    }
}