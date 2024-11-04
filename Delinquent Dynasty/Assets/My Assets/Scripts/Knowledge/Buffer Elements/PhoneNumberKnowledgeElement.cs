using Unity.Entities;

public struct PhoneNumberKnowledgeElement : IBufferElementData {
    public Entity Phone;
    public Entity KnowingEntity;
    public KnowledgeTimestamp Timestamp;
}