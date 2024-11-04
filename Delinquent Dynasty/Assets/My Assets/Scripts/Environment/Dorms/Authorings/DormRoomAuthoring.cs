using Unity.Entities;
using UnityEngine;

public class DormRoomAuthoring : MonoBehaviour { }

public class DormBaker : Baker<DormRoomAuthoring> {
    public override void Bake(DormRoomAuthoring authoring){
        AddComponent(new DormRoomComponent());
        AddComponent(new RoomComponent());
    }
}