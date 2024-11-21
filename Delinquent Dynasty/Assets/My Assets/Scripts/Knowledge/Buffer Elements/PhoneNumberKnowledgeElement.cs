using Unity.Entities;

[InternalBufferCapacity(0)]
public struct PhoneNumberKnowledgeElement : IBufferElementData {
    public Entity Phone;
    public EventTimestamp Timestamp;
}