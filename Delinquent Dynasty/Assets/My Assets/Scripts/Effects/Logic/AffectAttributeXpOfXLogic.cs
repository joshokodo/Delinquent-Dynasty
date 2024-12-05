using Unity.Collections;
using Unity.Entities;

public struct AffectAttributeXpOfXLogic : IApplyActiveEffect {
    [ReadOnly] public bool Display;
    [ReadOnly] public PassiveEffectsUtils PrimaryPassivesUtil;
    public DynamicBuffer<CharacterAttributeElement> PrimaryAttributes;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplayDamageSpawn;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue, out CharacterStateChangeSpawnElement primaryStateChange, out CharacterStateChangeSpawnElement secondaryStateChange,
        out CharacterStateChangeSpawnElement tertiaryStateChange, Entity secondaryTarget = default, Entity tertiaryTarget = default){
        primaryStateChange = default;
        secondaryStateChange = default;
        tertiaryStateChange = default;
        
        var number =
            PrimaryPassivesUtil.OnAffectAttributeXp(data, nextIntValue, sourceEntity, primaryTarget,
                ActiveEffectsSpawn);
        var attributesUtils = new CharacterAttributesUtil(){
            Attributes = PrimaryAttributes
        };

        var result = attributesUtils.AddXp(number, data.PrimaryEnumValue.AttributeType);
        primaryStateChange.AttributesChanged = true;
        if (result){
            primaryStateChange.AttributeTypeLeveledUp = data.PrimaryEnumValue.AttributeType;
            primaryStateChange.AttributeHasLeveledUp = true;
        }

        // if (Display){
        //     DisplayDamageSpawn.Add(new DisplayDamageSpawnElement(){
        //         CharacterEntity = primaryTarget,
        //         Text = new FixedString64Bytes(data.PrimaryEnumValue.AttributeType.ToString()),
        //         Value = number
        //     });
        // }
    }
}