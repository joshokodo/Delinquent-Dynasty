public struct XIsNaturalAttributeLevelConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var attributesUtil = new CharacterAttributesUtil(){
            Attributes = utils.AttributesLookup[target]
        };
        var currentLevel = attributesUtil.GetLevel(conditionData.PrimaryEnumValue.AttributeType);
        var value = (int) conditionData.PrimaryNumberValue;
        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, currentLevel, value) ==
               conditionData.ExpectedConditionValue;
    }
}