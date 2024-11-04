using Unity.Entities;
using UnityEngine;

public struct XIsAtRoomYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var room = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        var isAtRoom = utils.CharacterStateLookup.TryGetComponent(target, out CharacterWorldStateComponent state) &&
                       state.CurrentRoomEntity == room;

        return isAtRoom == conditionData.ExpectedConditionValue;
    }
}