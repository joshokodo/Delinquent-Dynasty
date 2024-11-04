using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct TravelToInteractableLocomotionJob : IJobEntity {
    public DynamicActionType DynamicActionType;
    public EntityCommandBuffer Ecb;
    public bool GoToOccupyingPoint;
    public TargetType TargetInteractable;

    [ReadOnly] public ActionDataStore DataStore;
    [ReadOnly] public ComponentLookup<InteractableLocationComponent> InteractableLocationComponentLookup;
    public ComponentLookup<CharacterWorldStateComponent> WorldStateLookup;
    public ComponentLookup<AgentBody> AgentBodyLookup;
    public ComponentLookup<AgentLocomotion> AgentLocomotionLookup;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = DataStore
        };

        var trip = new LocomotionUtils();

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets, out ActiveActionElement act,
                out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            if (actionUtils.TryGetTargetInteractableEntity(TargetInteractable, activeTargets,
                    out Entity interactablelocationEntity)){
                var interactableLocationComponent = InteractableLocationComponentLookup[interactablelocationEntity];
                var loc = GoToOccupyingPoint
                    ? interactableLocationComponent.OccupyingEntryLocation
                    : interactableLocationComponent.Location;
                
                var body = AgentBodyLookup[comp.CharacterEntity];
                var loco = AgentLocomotionLookup[comp.CharacterEntity];
                var worldState = WorldStateLookup[comp.CharacterEntity];
                
                
                trip.StartLocomotion(act, actions, index, loc, 0.5f, body, loco, worldState, Ecb, comp.CharacterEntity);

                if (trip.HasReachedDestination(body, 0.5f)){
                    actionUtils.StartPhase(DynamicActionType, Ecb, e, 1);
                }
                else{
                    //TODO: maybe auto cancel
                    // TODO: let player/AI know they cannot reach destination
                }
            }
        }
    }
}