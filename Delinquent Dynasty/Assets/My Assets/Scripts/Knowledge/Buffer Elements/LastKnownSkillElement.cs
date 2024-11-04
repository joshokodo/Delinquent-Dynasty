using Unity.Entities;

[InternalBufferCapacity(0)]
public struct LastKnownSkillElement : IBufferElementData {
    public SkillType SkillType;
    public int Value;
    public KnowledgeTimestamp Timestamp;
}