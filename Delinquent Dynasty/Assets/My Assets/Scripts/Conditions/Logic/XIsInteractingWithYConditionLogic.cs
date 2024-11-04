using Unity.Entities;

public struct XIsInteractingWithYConditionLogic : IConditionCheck {
    public bool Result(ConditionUtils utils, ConditionData conditionData){
        var x = utils.TargetsGroup.GetPrimaryTargetEntity(conditionData);
        var y = utils.TargetsGroup.GetSecondaryTargetEntity(conditionData);
        if (utils.CharacterStateLookup.TryGetComponent(x, out CharacterWorldStateComponent characterStateComponent)){
            var isInteracting = y == characterStateComponent.InteractableEntity || y == x;
            if (!isInteracting){
                if (characterStateComponent.InteractableEntity != Entity.Null && utils.ItemsLookup.TryGetBuffer(
                        characterStateComponent.InteractableEntity,
                        out DynamicBuffer<ItemElement> intereactingInventory)){
                    if (utils.LockItemSocketLookup.TryGetComponent(characterStateComponent.InteractableEntity,
                            out SecurityLockSocket lockSocket) && lockSocket.LockEntity == y){
                        isInteracting = true;
                    }

                    if (!isInteracting){
                        foreach (var itemElement in intereactingInventory){
                            if (utils.ItemsLookup.HasBuffer(itemElement.ItemEntity) && (itemElement.ItemEntity == y ||
                                    (utils.LockItemSocketLookup.TryGetComponent(itemElement.ItemEntity,
                                        out SecurityLockSocket socket) && socket.LockEntity == y))){
                                isInteracting = true;
                                break;
                            }
                        }
                    }
                }

                if (!isInteracting){
                    var xItems = utils.ItemsLookup[x];
                    foreach (var itemElement in xItems){
                        if (utils.ItemsLookup.HasBuffer(itemElement.ItemEntity) && itemElement.ItemEntity == y){
                            isInteracting = true;
                            break;
                        }
                    }
                }
            }

            return isInteracting == conditionData.ExpectedConditionValue;
        }

        return !conditionData.ExpectedConditionValue;
    }
}