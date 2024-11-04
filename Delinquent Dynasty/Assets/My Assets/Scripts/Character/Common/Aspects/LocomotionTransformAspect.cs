using System.Collections;
using System.Collections.Generic;
using ProjectDawn.Navigation;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct LocomotionTransformAspect : IAspect {
    public readonly RefRW<AgentBody> AgentBody;
    public readonly RefRW<LocalTransform> Transform;
    public readonly RefRW<AgentLocomotion> AgentLocomotion;

    public void SetStaticPosition(InteractableLocationComponent interactableLocationComponent){
        Transform.ValueRW.Position = interactableLocationComponent.OccupyingPosition;
        Transform.ValueRW.Rotation = interactableLocationComponent.OccupyingRotation;
    }
}