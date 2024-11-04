using UnityEngine;

public struct IsClassPeriodInSessionConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        switch (conditionData.PrimaryNumberValue){
            case 1:
                return conditionData.ExpectedConditionValue == utils.InGameTime.IsPeriod1();
            case 2:
                return conditionData.ExpectedConditionValue == utils.InGameTime.IsPeriod2();
            case 3:
                return conditionData.ExpectedConditionValue == utils.InGameTime.IsPeriod3();
            case 4:
                return conditionData.ExpectedConditionValue == utils.InGameTime.IsPeriod4();
            case 5:
                return conditionData.ExpectedConditionValue == utils.InGameTime.IsPeriod5();
            case 6:
                return conditionData.ExpectedConditionValue == utils.InGameTime.IsPeriod6();
        }

        return !conditionData.ExpectedConditionValue;
    }
}