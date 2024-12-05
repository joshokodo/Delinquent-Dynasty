using Unity.Entities;
using UnityEngine;

public struct AffectTraitIntensityOfXLogic : IApplyActiveEffect {
    public DynamicBuffer<TraitElement> Traits;
    public TraitUtils TraitUtils;
    public DynamicBuffer<PassiveEffectSpawnElement> PassiveEffectSpawnElements;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplaySpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;
    public bool Display;
    public InGameTime InGameTime;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        var added = TraitUtils.AddTrait(primaryTarget, PassiveEffectSpawnElements, data.PrimaryEnumValue.GetTraitType(),
            nextIntValue, Traits, InGameTime, sourceEntity);

        if (added){
            primaryStateChange.TraitsChanged = true;
        }
        
        if (Display && data.PrimaryEnumValue.GetTraitType().Category == TraitCategory.STATUS && nextIntValue > 0){
            DisplaySpawnElements.Add(new DisplayDamageSpawnElement(){
                CharacterEntity = primaryTarget,
                DisplayEnum = data.PrimaryEnumValue,
                DisplayColor = Color.yellow
            });
        }

        StateChangeSpawn.Add(new CharacterStateChangeSpawnElement(){
            Character = primaryTarget,
            TraitsChanged = true,
        });
    }
}