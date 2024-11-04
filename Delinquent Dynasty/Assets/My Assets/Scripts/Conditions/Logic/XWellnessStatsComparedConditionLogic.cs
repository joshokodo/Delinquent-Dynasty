public struct XWellnessStatsComparedConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        //TODO: important add passive utils to get bonus wellness ect. do for all these conditions
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var wellnessUtils = new WellnessUtils(){
            Wellness = utils.WellnessLookup[target]
        };
        var wellnessSubject =
            wellnessUtils.GetCurrentValue(conditionData.PrimaryEnumValue.WellnessType);
        var wellnessCompare =
            wellnessUtils.GetCurrentValue(conditionData.SecondaryEnumValue.WellnessType);
        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, wellnessSubject,
            wellnessCompare) == conditionData.ExpectedConditionValue;
    }
}