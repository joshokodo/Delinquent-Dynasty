using Unity.Entities;

[InternalBufferCapacity(0)]
public struct LastKnownTraitElement : IBufferElementData {
    public DynamicTraitType TraitType;
    public int Value;
    public EventTimestamp Timestamp;
}