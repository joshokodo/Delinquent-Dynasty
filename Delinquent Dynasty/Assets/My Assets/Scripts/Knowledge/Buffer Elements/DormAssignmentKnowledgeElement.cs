using Unity.Entities;

[InternalBufferCapacity(2)]
public struct DormAssignmentKnowledgeElement : IBufferElementData {
    public Entity BedEntity;
    public Entity AssignedCharacterEntity;
}