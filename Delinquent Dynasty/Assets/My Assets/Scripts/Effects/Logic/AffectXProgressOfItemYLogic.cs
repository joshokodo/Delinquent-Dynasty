using Unity.Collections;
using Unity.Entities;

public struct AffectXProgressOfItemYLogic : IApplyActiveEffect {
    public DynamicBuffer<CharacterKnowledgeElement> PrimaryKnowledge;
    public DynamicBuffer<KnowledgeSpawnElement> KnowledgeSpawn;
    [ReadOnly] public ComponentLookup<ItemProgressKnowledgeComponent> ItemProgressLookup;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        out CharacterStateChangeSpawnElement primaryStateChange,
        out CharacterStateChangeSpawnElement secondaryStateChange,
        Entity secondaryTarget = default){
        primaryStateChange = default;
        secondaryStateChange = default;
    }
}