using Unity.Collections;
using Unity.Entities;

public struct RoomComponent : IComponentData {
    public FixedString128Bytes RoomName;
    public RoomType RoomType;
    public Entity BuildingEntity;
    public int FloorNumber;
}