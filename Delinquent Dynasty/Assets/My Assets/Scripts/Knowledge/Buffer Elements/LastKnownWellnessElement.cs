using Unity.Entities;

[InternalBufferCapacity(0)]
public struct LastKnownWellnessElement : IBufferElementData {
    public WellnessType WellnessType;
    public int Value;
    public KnowledgeTimestamp Timestamp;
}