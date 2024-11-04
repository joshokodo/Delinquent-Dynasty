using Unity.Entities;
using UnityEngine;

public class BuildingHallwayAuthoring : MonoBehaviour { }

public class BuildingHallwayBaker : Baker<BuildingHallwayAuthoring> {
    public override void Bake(BuildingHallwayAuthoring authoring){
        AddComponent(new RoomComponent());
    }
}