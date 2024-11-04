using Unity.Entities;
using UnityEngine;

public class ActiveEffectAuthoring : MonoBehaviour { }

public class ActiveEffectBaker : Baker<ActiveEffectAuthoring> {
    public override void Bake(ActiveEffectAuthoring authoring){
        AddComponent<ActiveEffectComponent>();
    }
}