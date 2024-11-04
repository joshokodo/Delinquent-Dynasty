using Unity.Entities;
using Unity.Transforms;

public readonly partial struct PassiveEffectAspect : IAspect {
    public readonly RefRW<PassiveEffectComponent> EffectComponent;
}