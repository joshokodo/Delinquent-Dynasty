public struct IsTimeFrameConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        return conditionData.ExpectedConditionValue ==
               utils.InGameTime.IsTimeFrame(conditionData.PrimaryNumberValue, conditionData.SecondaryNumberValue);
    }
}