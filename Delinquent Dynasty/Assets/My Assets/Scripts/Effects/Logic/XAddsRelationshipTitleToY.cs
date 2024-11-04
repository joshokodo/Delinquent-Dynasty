using Unity.Collections;
using Unity.Entities;

public struct XAddsRelationshipTitleToY : IApplyActiveEffect {
    public DynamicBuffer<RelationshipElement> PrimaryRelationships;

    public DynamicBuffer<PassiveEffectSpawnElement> PassivesSpawn;
    public CharactersDataStore CharacterDataStore;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        Entity secondaryTarget = default){
        new RelationshipUtils().AffectRelationshipTitle(primaryTarget, secondaryTarget, PassivesSpawn,
            CharacterDataStore, data.PrimaryEnumValue.RelationshipMainTitleType,
            PrimaryRelationships);
    }
}