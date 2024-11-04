using Unity.Burst;
using Unity.Collections;
using Unity.Entities;


[BurstCompile]
public partial struct SetBusinessJob : IJobEntity {
    [ReadOnly] public ActionDataStore ActionDataStore;
    public bool SetValue;
    public DynamicActionType ActionType;

    public ComponentLookup<CommercableLocationComponent> CommercableLookup;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawnElements;
    public EntityCommandBuffer Ecb;
    public int NextPhase;

    public void Execute(Entity e, DynamicBuffer<ActiveActionElement> actions, DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = ActionDataStore
        };

        if (actionUtils.TryGetActiveActionAndTargetEntity(ActionType, actions, targets, TargetType.TARGET_INVENTORY,
                out ActiveActionElement act, out Entity target, out int index)){
            if (CommercableLookup.TryGetComponent(target,
                    out CommercableLocationComponent commercableLocationComponent)){
                commercableLocationComponent.IsSelling = SetValue;
                Ecb.SetComponent(target, commercableLocationComponent);

                StateChangeSpawnElements.Add(new CharacterStateChangeSpawnElement(){
                    Character = e,
                    InventoryChanged = true,
                    PerformedSuccessfulAction = true
                });
            }

            if (NextPhase < 0){
                actionUtils.StartPhase(ActionType, Ecb, e, -1);
                actionUtils.CompleteAction(act, actions, index);
            }
            else{
                actionUtils.StartPhase(ActionType, Ecb, e, NextPhase);
            }
        }
    }
}