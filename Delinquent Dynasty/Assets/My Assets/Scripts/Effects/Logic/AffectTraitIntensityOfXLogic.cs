﻿using Unity.Entities;
using UnityEngine;

public struct AffectTraitIntensityOfXLogic : IApplyActiveEffect {
    public DynamicBuffer<TraitElement> Traits;
    public TraitUtils TraitUtils;
    public DynamicBuffer<PassiveEffectSpawnElement> PassiveEffectSpawnElements;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplaySpawnElements;
    public DynamicBuffer<CharacterStateChangeSpawnElement> StateChangeSpawn;
    public bool Display;
    public InGameTime InGameTime;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        Entity secondaryTarget = default){
        TraitUtils.AddTrait(primaryTarget, PassiveEffectSpawnElements, data.PrimaryEnumValue.GetTraitType(),
            nextIntValue, Traits, InGameTime, sourceEntity);

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