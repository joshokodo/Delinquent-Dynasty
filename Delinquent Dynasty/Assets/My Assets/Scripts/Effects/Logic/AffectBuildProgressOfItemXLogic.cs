using Unity.Entities;

public struct AffectBuildProgressOfItemXLogic : IApplyActiveEffect {
    public ComponentLookup<ItemBuildInProgressComponent> BuildInProgressLookup;
    public EntityCommandBuffer Ecb;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        Entity secondaryTarget = default){
        primaryStateChange = default;
        secondaryStateChange = default;
        if (BuildInProgressLookup.TryGetComponent(primaryTarget, out ItemBuildInProgressComponent buildComp)){
            buildComp.AddBuildPercentage(nextIntValue);
            if (buildComp.DefectPercentage >= 100){ }
            else if (buildComp.BuildPercentage >= 100){
                Ecb.AddComponent(primaryTarget, new TransformItemComponent(){
                    ItemType = buildComp.SuccessfulProduct
                });
            }
            else{
                Ecb.SetComponent(primaryTarget, buildComp);
            }
        }
    }
}