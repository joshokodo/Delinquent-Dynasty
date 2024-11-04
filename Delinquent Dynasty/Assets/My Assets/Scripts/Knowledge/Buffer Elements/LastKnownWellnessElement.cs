using Unity.Entities;

[InternalBufferCapacity(7)]
public struct LastKnownWellnessElement : IBufferElementData {
    public WellnessType WellnessType;
    public int Value;
    public KnowledgeTimestamp Timestamp;
}