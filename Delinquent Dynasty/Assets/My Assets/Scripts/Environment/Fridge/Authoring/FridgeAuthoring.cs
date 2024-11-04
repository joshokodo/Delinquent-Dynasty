using Unity.Entities;
using UnityEngine;

public class FridgeAuthoring : MonoBehaviour { }

public class FridgeBaker : Baker<FridgeAuthoring> {
    public override void Bake(FridgeAuthoring authoring){
        AddComponent(typeof(FridgeComponent));
        AddComponent(new InteractableInventoryComponent(){
            CarryLimit = 100,
            InventoryTempurature = TemperatureType.COLD
        });
        AddComponent(typeof(SecurityLockSocket));
        AddComponent(new InteractableLocationComponent());
        AddBuffer<ItemElement>();
    }
}