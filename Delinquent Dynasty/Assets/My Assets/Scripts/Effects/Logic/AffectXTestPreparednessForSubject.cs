using Unity.Entities;

public struct AffectXTestPreparednessForSubject : IApplyActiveEffect {
    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawnElements;
    public DynamicBuffer<CharacterKnowledgeElement> PrimaryKnowledge;
    public ComponentLookup<TestPreparednessKnowledgeComponent> TestPreparedKnowledgeCompLookup;
    public ComponentLookup<SourceTestKnowledgeComponent> SourceTestKnowledgeCompLookup;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
    }
}