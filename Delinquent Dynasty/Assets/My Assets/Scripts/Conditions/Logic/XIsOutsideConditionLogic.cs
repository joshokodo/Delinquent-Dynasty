using Unity.Entities;

public struct XIsOutsideConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        if (utils.CharacterStateLookup.TryGetComponent(x, out CharacterWorldStateComponent stateComponent)){
            return (stateComponent.CurrentRoomEntity == Entity.Null) == conditionData.ExpectedConditionValue;
        }

        return !conditionData.ExpectedConditionValue;
    }
}