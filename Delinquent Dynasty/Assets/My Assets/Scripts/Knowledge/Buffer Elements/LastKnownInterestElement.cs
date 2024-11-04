using Unity.Entities;

[InternalBufferCapacity(0)]
public struct LastKnownInterestElement : IBufferElementData {
    public InterestSubjectType SubjectType;
    public DynamicGameEnum PrimaryEnumValue;
    public int Value;
    public KnowledgeTimestamp Timestamp;
}