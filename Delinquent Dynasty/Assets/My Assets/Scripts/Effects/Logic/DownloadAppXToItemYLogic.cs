using Unity.Entities;

public struct DownloadAppXToItemYLogic : IApplyActiveEffect {
    public AppComponent PrimaryAppComponent;
    public DynamicBuffer<AppElement> SecondaryApps;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;

        var found = false;
        foreach (var secondaryApp in SecondaryApps){
            if (secondaryApp.AppName.Equals(PrimaryAppComponent.AppName)){
                found = true;
                break;
            }
        }

        if (!found){
            SecondaryApps.Add(new AppElement(){
                AppName = PrimaryAppComponent.AppName
            });
        }
    }
}