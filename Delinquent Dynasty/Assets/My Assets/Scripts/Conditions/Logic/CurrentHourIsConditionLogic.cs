public struct CurrentHourIsConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        return conditionData.ExpectedConditionValue == NumberUtils.CheckNumberComparision(
            conditionData.NumericComparisonSign, (int) utils.InGameTime.CurrentDayHours,
            (int) conditionData.PrimaryNumberValue);
    }
}