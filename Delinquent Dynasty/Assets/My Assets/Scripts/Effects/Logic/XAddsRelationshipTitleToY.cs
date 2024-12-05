using Unity.Collections;
using Unity.Entities;

public struct XAddsRelationshipTitleToY : IApplyActiveEffect {
    public DynamicBuffer<RelationshipElement> PrimaryRelationships;

    public DynamicBuffer<PassiveEffectSpawnElement> PassivesSpawn;
    public CharactersDataStore CharacterDataStore;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        primaryStateChange.RelationshipChanged = true;
        secondaryStateChange.RelationshipChanged = true;
        
        new RelationshipUtils().AffectRelationshipTitle(primaryTarget, secondaryTarget, PassivesSpawn,
            CharacterDataStore, data.PrimaryEnumValue.RelationshipMainTitleType,
            PrimaryRelationships);
    }
}