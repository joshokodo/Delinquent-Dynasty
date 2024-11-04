using Unity.Collections;
using Unity.Entities;

public struct AffectAttributeXpOfXLogic : IApplyActiveEffect {
    [ReadOnly] public bool Display;
    [ReadOnly] public PassiveEffectsUtils PrimaryPassivesUtil;
    public DynamicBuffer<CharacterAttributeElement> PrimaryAttributes;
    public DynamicBuffer<ActiveEffectSpawnElement> ActiveEffectsSpawn;
    public DynamicBuffer<DisplayDamageSpawnElement> DisplayDamageSpawn;

    public void Apply(Entity sourceEntity, Entity primaryTarget, ActiveEffectData data, int nextIntValue,
        Entity secondaryTarget = default){
        var number =
            PrimaryPassivesUtil.OnAffectAttributeXp(data, nextIntValue, sourceEntity, primaryTarget,
                ActiveEffectsSpawn);
        var attributesUtils = new CharacterAttributesUtil(){
            Attributes = PrimaryAttributes
        };

        var result = attributesUtils.AddXp(number, data.PrimaryEnumValue.AttributeType);

        // if (Display){
        //     DisplayDamageSpawn.Add(new DisplayDamageSpawnElement(){
        //         CharacterEntity = primaryTarget,
        //         Text = new FixedString64Bytes(data.PrimaryEnumValue.AttributeType.ToString()),
        //         Value = number
        //     });
        // }
    }
}