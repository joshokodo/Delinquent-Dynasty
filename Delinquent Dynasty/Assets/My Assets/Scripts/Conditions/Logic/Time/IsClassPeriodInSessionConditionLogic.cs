using UnityEngine;

public struct IsClassPeriodInSessionConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){

        var val = 0;
        if (utils.InGameTime.IsPeriod1()){
            val = 1;
        } else if (utils.InGameTime.IsPeriod2()){
            val = 2;
        } else if (utils.InGameTime.IsPeriod3()){
            val = 3;
        } else if (utils.InGameTime.IsPeriod4()){
            val = 4;
        } else if (utils.InGameTime.IsPeriod5()){
            val = 5;
        } else if (utils.InGameTime.IsPeriod6()){
            val = 6;
        }

        return NumberUtils.CheckNumberComparision(conditionData.NumericComparisonSign, val, (int) conditionData.PrimaryNumberValue) == conditionData.ExpectedConditionValue;
    }
}