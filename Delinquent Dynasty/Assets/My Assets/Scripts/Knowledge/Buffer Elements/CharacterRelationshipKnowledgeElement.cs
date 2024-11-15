using Unity.Entities;

[InternalBufferCapacity(0)]
public struct CharacterRelationshipKnowledgeElement : IBufferElementData {
    public Entity KnowledgeEntity;
    public Entity CharacterEntity;
}