using Unity.Entities;

public struct ItemXIsLockedConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var isLocked = false;
        var target = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        if (target != Entity.Null){
            if (utils.ItemSecurityLockLookup.TryGetComponent(target, out ItemSecurityLockComponent lockComponent)){
                isLocked = lockComponent.IsLocked;
            }
            else if (utils.LockItemSocketLookup.TryGetComponent(target, out SecurityLockSocket lockItemSocket)
                     && utils.ItemSecurityLockLookup.TryGetComponent(lockItemSocket.LockEntity,
                         out ItemSecurityLockComponent childLockComponent)){
                isLocked = childLockComponent.IsLocked;
            }
        }

        return isLocked == conditionData.ExpectedConditionValue;
    }
}