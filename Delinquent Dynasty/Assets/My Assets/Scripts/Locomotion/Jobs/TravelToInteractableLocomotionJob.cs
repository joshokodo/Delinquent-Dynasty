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

    [ReadOnly] public ComponentLookup<InteractableInventoryComponent> InventoryTagLookup;
    [ReadOnly] public ComponentLookup<DoorTag> DoorTagLookup;
    [ReadOnly] public ComponentLookup<SinkTag> SinkTagLookup;
    [ReadOnly] public ComponentLookup<ToiletTag> ToiletTagLookup;

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
            if (actionUtils.TryGetTargetInteractableEntity(TargetInteractable, activeTargets,
                    out Entity interactablelocationEntity)){
                var interactableLocationComponent = InteractableLocationComponentLookup[interactablelocationEntity];
                var loc = GoToOccupyingPoint
                    ? interactableLocationComponent.OccupyingEntryLocation
                    : interactableLocationComponent.Location;

                var body = AgentBodyLookup[comp.CharacterEntity];
                var loco = AgentLocomotionLookup[comp.CharacterEntity];
                var worldState = WorldStateLookup[comp.CharacterEntity];


                var hasStarted = trip.StartLocomotion(act, actions, index, loc, 0.5f, body, loco, worldState, Ecb,
                    comp.CharacterEntity);

                if (hasStarted){
                    return;
                }

                if (trip.HasReachedDestination(body, 0.5f) && trip.HasReachedEndOfPath(body, 0.5f)){
                    if (interactableLocationComponent.IsInInteractingQueue(comp.CharacterEntity, out bool isInFront)){
                        if (isInFront){
                        
                            worldState.LocomotionState = FinalLocomotionState;
                            worldState.InteractableEntity = interactablelocationEntity;

                            Ecb.SetComponent(comp.CharacterEntity, worldState);

                            if (OccupyLocation){
                                var transform = TransformLookup[comp.CharacterEntity];
                                transform.Position = interactableLocationComponent.OccupyingPosition;
                                transform.Rotation = interactableLocationComponent.OccupyingRotation;
                                Ecb.SetComponent(comp.CharacterEntity, transform);
                            }

                            Ecb.SetComponent(interactablelocationEntity, interactableLocationComponent);

                            // todo replace with stateChangeSpawnElement and do this logic there
                            if (SelectedLookup.TryGetComponent(comp.CharacterEntity, out SelectedCharacter tag)){
                                if (InventoryTagLookup.HasComponent(interactablelocationEntity) &&
                                    !tag.ShowInventoryUI){
                                    tag.ShowInventoryUI = true;
                                    Ecb.SetComponent(comp.CharacterEntity, tag);
                                }
                                else if (!tag.ShowInteractableUI){
                                    if (DoorTagLookup.HasComponent(interactablelocationEntity)){
                                        tag.ShowInteractableUI = true;
                                        tag.InteractableTypeUI = InteractableType.DOOR;
                                    }
                                    else if (SinkTagLookup.HasComponent(interactablelocationEntity)){
                                        tag.ShowInteractableUI = true;
                                        tag.InteractableTypeUI = InteractableType.SINK;
                                    }
                                    else if (ToiletTagLookup.HasComponent(interactablelocationEntity)){
                                        tag.ShowInteractableUI = true;
                                        tag.InteractableTypeUI = InteractableType.TOILET;
                                    }

                                    Ecb.SetComponent(comp.CharacterEntity, tag);
                                }
                            }
                            
                            actionUtils.StartPhase(DynamicActionType, Ecb, e, 1);
                        }
                        else{
                            // chillin
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