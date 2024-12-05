using System;

public struct FlatSuccessChanceConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var isSuccessful = utils.RandomComponent.ValueRW.IsSuccessful((int) conditionData.PrimaryNumberValue);
        return isSuccessful == conditionData.ExpectedConditionValue;
    }
}