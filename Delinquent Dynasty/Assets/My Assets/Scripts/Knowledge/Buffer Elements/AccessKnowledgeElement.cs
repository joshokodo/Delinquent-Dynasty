using Unity.Entities;

[InternalBufferCapacity(0)]
public struct AccessKnowledgeElement : IBufferElementData {
    public Entity KnowledgeEntity;
}