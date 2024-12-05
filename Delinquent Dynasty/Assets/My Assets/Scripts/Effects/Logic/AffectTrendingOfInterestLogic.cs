using Unity.Entities;

public struct AffectTrendingOfInterestLogic : IApplyActiveEffect {
    public DynamicBuffer<AffectTrendingSpawnElement> AffectTrendingSpawn;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        AffectTrendingSpawn.Add(new AffectTrendingSpawnElement(){
            TargetEnum = new DynamicGameEnum(){
                Type = data.SecondaryEnumValue.Type,
                IntValue = data.SecondaryEnumValue.IntValue
            },
            Value = nextIntValue
        });
    }
}