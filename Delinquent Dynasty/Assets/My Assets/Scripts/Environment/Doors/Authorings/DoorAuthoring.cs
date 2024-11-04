using Unity.Entities;
using UnityEngine;

public class DoorAuthoring : MonoBehaviour { }

public class DoorBaker : Baker<DoorAuthoring> {
    public override void Bake(DoorAuthoring authoring){
        AddComponent(typeof(SecurityLockSocket));
        AddComponent(typeof(InteractableLocationComponent));
        AddComponent(typeof(DoorTag));
    }
}