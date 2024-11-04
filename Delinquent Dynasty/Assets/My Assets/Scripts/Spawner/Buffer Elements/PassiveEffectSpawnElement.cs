using Unity.Entities;

public struct PassiveEffectSpawnElement : IBufferElementData {
    public PassiveEffectComponent Data;
    public Entity Entity;
}