using Unity.Entities;

public struct EffectResultData {
    public ActiveEffectType ActiveEffectType;
    public DynamicGameEnum PrimaryEnumValue;
    public DynamicGameEnum SecondaryEnumValue;

    public int TotalEffectValue;

    public Entity EffectTargetCharacter;
    public Entity EffectOriginCharacter;
}