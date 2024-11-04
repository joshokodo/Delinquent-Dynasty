using Unity.Entities;
using UnityEngine;

public class StoveAuthoring : MonoBehaviour { }

public class StoveBaker : Baker<StoveAuthoring> {
    public override void Bake(StoveAuthoring authoring){
        AddComponent(typeof(StoveComponent));
        AddComponent(new InteractableInventoryComponent(){
            CarryLimit = 50,
            InventoryTempurature = TemperatureType.MID
        });
        AddComponent(new InteractableLocationComponent());
        AddBuffer<ItemElement>();
    }
}