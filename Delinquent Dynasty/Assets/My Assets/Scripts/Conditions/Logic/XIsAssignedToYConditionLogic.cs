public struct XIsAssignedToYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var y = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        var isAssigned = false;
        if (utils.BedLookup.TryGetComponent(x, out BedComponent bedComp)){
            isAssigned = bedComp.AssignedCharacterEntity == y;
        }
        else if (utils.LockerLookup.TryGetComponent(x, out LockerComponent lockerComp)){
            isAssigned = lockerComp.AssignedCharacter == y;
        }
        else if (utils.DormRoomLookup.TryGetComponent(x, out DormRoomComponent dormComp)){
            var bed1 = utils.BedLookup[dormComp.BedEntity1];
            isAssigned = bed1.AssignedCharacterEntity == y;
            if (!isAssigned){
                var bed2 = utils.BedLookup[dormComp.BedEntity2];
                isAssigned = bed2.AssignedCharacterEntity == y;
            }
        }

        return isAssigned == conditionData.ExpectedConditionValue;
    }
}