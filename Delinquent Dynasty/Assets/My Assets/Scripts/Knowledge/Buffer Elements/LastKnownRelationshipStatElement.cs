using Unity.Entities;

public struct LastKnownRelationshipStatElement : IBufferElementData {
    public RelationshipStatType RelationshipStatType;
    public int Value;
    public KnowledgeTimestamp Timestamp;
}