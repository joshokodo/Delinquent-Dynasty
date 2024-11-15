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

    [ReadOnly] public ActionDataStore DataStore;
    [ReadOnly] public ComponentLookup<InteractableLocationComponent> InteractableLocationComponentLookup;
    public ComponentLookup<CharacterWorldStateComponent> WorldStateLookup;
    public ComponentLookup<AgentBody> AgentBodyLookup;
    public ComponentLookup<AgentLocomotion> AgentLocomotionLookup;
    // public ComponentLookup<NavMeshPath> PathLookup;

    [ReadOnly] public ComponentLookup<InteractableInventoryComponent> InventoryTagLookup;

    [ReadOnly] public ComponentLookup<SelectedCharacter> SelectedLookup;
    public ComponentLookup<LocalTransform> TransformLookup;

    public LocomotionState FinalLocomotionState;
    public bool OccupyLocation;

    public void Execute(Entity e, CharacterBehaviorEntityComponent comp, DynamicBuffer<ActiveActionElement> actions,
        DynamicBuffer<ActiveActionTargetElement> targets){
        var actionUtils = new ActionUtils(){
            ActionDataStore = DataStore
        };

        var trip = new LocomotionUtils();

        if (actionUtils.TryGetActiveActionAndTargets(DynamicActionType, actions, targets, out ActiveActionElement act,
                out FixedList4096Bytes<ActiveActionTargetElement> activeTargets, out int index)){
            if (actionUtils.TryGetTargetInteractableEntity(TargetType.TARGET_INTERACTABLE, activeTargets,
                    out Entity interactablelocationEntity)){
                var interactableLocationComponent = InteractableLocationComponentLookup[interactablelocationEntity];
                var loc = GoToOccupyingPoint
                    ? interactableLocationComponent.OccupyingEntryLocation
                    : interactableLocationComponent.Location;

                var body = AgentBodyLookup[comp.CharacterEntity];
                var loco = AgentLocomotionLookup[comp.CharacterEntity];
                // var path = PathLookup[comp.CharacterEntity];
                var worldState = WorldStateLookup[comp.CharacterEntity];

                var hasStarted = trip.StartLocomotion(act, actions, index, loc, 0.5f, body, loco, worldState, Ecb,
                    comp.CharacterEntity);

                if (hasStarted){
                    return;
                }

                if (trip.HasReachedEndOfPath(body, 0.5f)){
                    if (interactableLocationComponent.IsInInteractingQueue(comp.CharacterEntity, out bool isInFront, out int queueIndex)){
                        if (isInFront){

                            if (worldState.LocomotionState != FinalLocomotionState ||
                                worldState.InteractableEntity != interactablelocationEntity || worldState.PositionInQueue != queueIndex){
                                worldState.LocomotionState = FinalLocomotionState;
                                worldState.InteractableEntity = interactablelocationEntity;
                                worldState.PositionInQueue = queueIndex;
                                Ecb.SetComponent(comp.CharacterEntity, worldState);
                            }
           

                            if (OccupyLocation){
                                var transform = TransformLookup[comp.CharacterEntity];
                                if (!transform.Position.Equals(interactableLocationComponent.OccupyingPosition)
                                    || !transform.Rotation.Equals(interactableLocationComponent.OccupyingRotation)){
                                    transform.Position = interactableLocationComponent.OccupyingPosition;
                                    transform.Rotation = interactableLocationComponent.OccupyingRotation;
                                    Ecb.SetComponent(comp.CharacterEntity, transform);
                                }
                           
                            }

                            if (SelectedLookup.TryGetComponent(comp.CharacterEntity, out SelectedCharacter tag)){
                                if (InventoryTagLookup.HasComponent(interactablelocationEntity) &&
                                    !tag.ShowInventoryUI){
                                    tag.ShowInventoryUI = true;
                                    Ecb.SetComponent(comp.CharacterEntity, tag);
                                }
                                else if (!tag.ShowInteractableUI){
                                    switch (interactableLocationComponent.InteractableType){
                                        case InteractableType.DOOR:
                                            tag.ShowInteractableUI = true;
                                            tag.InteractableTypeUI = InteractableType.DOOR;
                                            break;
                                        
                                        case InteractableType.SINK:
                                            tag.ShowInteractableUI = true;
                                            tag.InteractableTypeUI = InteractableType.SINK;
                                            break;
                                        
                                        case InteractableType.TOILET:
                                            tag.ShowInteractableUI = true;
                                            tag.InteractableTypeUI = InteractableType.TOILET;
                                            break;
                                    }
                                  
                                    Ecb.SetComponent(comp.CharacterEntity, tag);
                                }
                            }
                            
                        }
                        else{
                            if (worldState.LocomotionState != FinalLocomotionState ||
                                worldState.InteractableEntity != interactablelocationEntity || worldState.PositionInQueue != queueIndex){
                                worldState.LocomotionState = FinalLocomotionState;
                                worldState.InteractableEntity = interactablelocationEntity;
                                worldState.PositionInQueue = queueIndex;
                                Ecb.SetComponent(comp.CharacterEntity, worldState);
                            }

                            if (SelectedLookup.TryGetComponent(comp.CharacterEntity, out SelectedCharacter tag)){

                                if (InventoryTagLookup.HasComponent(interactablelocationEntity) &&
                                    !tag.ShowInventoryUI){
                                    tag.ShowInventoryUI = true;
                                    Ecb.SetComponent(comp.CharacterEntity, tag);
                                }
                                else if (!tag.ShowInteractableUI){
                                    switch (interactableLocationComponent.InteractableType){
                                        case InteractableType.DOOR:
                                            tag.ShowInteractableUI = true;
                                            tag.InteractableTypeUI = InteractableType.DOOR;
                                            break;
                                        
                                        case InteractableType.SINK:
                                            tag.ShowInteractableUI = true;
                                            tag.InteractableTypeUI = InteractableType.SINK;
                                            break;
                                        
                                        case InteractableType.TOILET:
                                            tag.ShowInteractableUI = true;
                                            tag.InteractableTypeUI = InteractableType.TOILET;
                                            break;
                                    }
                                    Ecb.SetComponent(comp.CharacterEntity, tag);
                                } 
                            }
                        }
                    }
                    else{
                        interactableLocationComponent.InteractingEntities.Add(comp.CharacterEntity);
                        Ecb.SetComponent(interactablelocationEntity, interactableLocationComponent);
                    }
                } else {
                    //TODO: maybe auto cancel
                    // TODO: let player/AI know they cannot reach destination
                }
            }
        }
    }
}