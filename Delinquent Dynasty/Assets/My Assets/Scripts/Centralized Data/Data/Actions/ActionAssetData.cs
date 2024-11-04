using System;
using Unity.Collections;

public struct ActionAssetData {
    public Guid ParentId;
    public Guid ActionId;
    public DynamicActionType ActionType;
    public bool DeleteOnCompletion;
    public bool DeleteOnDeactivation;
    public int Iterations;
    public bool AnyCondition;

    public ActionElement ToBufferElement(){
        return new ActionElement(){
            ActionType = ActionType,
            ActionId = ActionId,
            DeleteOnCompletion = DeleteOnCompletion,
            DeleteOnDeactivation = DeleteOnDeactivation,
            TotalIterations = Iterations,
            AnyCondition = AnyCondition,
        };
    }
}