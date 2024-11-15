using Unity.Entities;

[InternalBufferCapacity(0)]
public struct DormAssignmentKnowledgeElement : IBufferElementData {
    public Entity BedEntity;
    public Entity AssignedCharacterEntity;
}