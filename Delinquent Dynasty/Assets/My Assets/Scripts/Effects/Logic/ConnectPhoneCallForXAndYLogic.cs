using Unity.Entities;

public struct ConnectPhoneCallForXAndYLogic : IApplyActiveEffect {
    public ComponentLookup<ItemCellPhoneComponent> CellLookup;
    public EntityCommandBuffer Ecb;
    
    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){

        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        var primaryCell = CellLookup[primaryTarget];
        var secondaryCell = CellLookup[secondaryTarget];
        
        primaryCell.PhoneCallWith = secondaryTarget;
        secondaryCell.PhoneCallWith = primaryTarget;
        
        Ecb.SetComponent(primaryTarget, primaryCell);
        Ecb.SetComponent(secondaryTarget, secondaryCell);
    }
}