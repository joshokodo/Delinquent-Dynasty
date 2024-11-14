using Unity.Entities;

[InternalBufferCapacity(0)] 
public struct RemoveFromQueueSpawnElement : IBufferElementData {
    public Entity Interactable;
    public Entity Character;
}