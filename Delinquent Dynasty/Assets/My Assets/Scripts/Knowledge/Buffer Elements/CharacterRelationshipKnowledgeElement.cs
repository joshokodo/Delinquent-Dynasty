using Unity.Entities;

public struct CharacterRelationshipKnowledgeElement : IBufferElementData {
    public Entity KnowledgeEntity;
    public Entity CharacterEntity;
}