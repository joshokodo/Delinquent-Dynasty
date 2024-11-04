using Unity.Entities;
using Unity.Transforms;

public readonly partial struct ActiveEffectElementAspect : IAspect {
    public readonly RefRW<ActiveEffectComponent> EffectComponent;
}