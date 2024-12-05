using Unity.Entities;

public struct CreateAccountForXOnAppYLogic : IApplyActiveEffect {

    public DynamicBuffer<AppUserSpawnElement> AppUserSpawn;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        AppUserSpawn.Add(new AppUserSpawnElement(){
            TargetEntity = primaryTarget,
            TargetApp = secondaryTarget
        });
    }
}