using Unity.Entities;

public struct DormRoomComponent : IComponentData {
    public bool IsFemaleRoom;
    public Entity BedEntity1;
    public Entity BedEntity2;
    public Entity DoorEntity;
}