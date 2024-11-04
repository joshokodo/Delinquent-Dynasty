using Unity.Entities;

public struct AffectTrendingOfInterestLogic : IApplyActiveEffect {
    public DynamicBuffer<AffectTrendingSpawnElement> AffectTrendingSpawn;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        Entity secondaryTarget = default){
        AffectTrendingSpawn.Add(new AffectTrendingSpawnElement(){
            TargetEnum = new DynamicGameEnum(){
                Type = data.SecondaryEnumValue.Type,
                IntValue = data.SecondaryEnumValue.IntValue
            },
            Value = nextIntValue
        });
    }
}