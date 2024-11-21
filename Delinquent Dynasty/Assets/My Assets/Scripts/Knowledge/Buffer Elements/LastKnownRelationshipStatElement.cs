using Unity.Entities;

[InternalBufferCapacity(0)]
public struct LastKnownRelationshipStatElement : IBufferElementData {
    public RelationshipStatType RelationshipStatType;
    public int Value;
    public EventTimestamp Timestamp;
}