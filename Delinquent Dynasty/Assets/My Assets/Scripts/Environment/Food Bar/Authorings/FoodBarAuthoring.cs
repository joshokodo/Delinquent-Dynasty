using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class FoodBarAuthoring : MonoBehaviour { }

public class FoodBarBaker : Baker<FoodBarAuthoring> {
    public override void Bake(FoodBarAuthoring authoring){
        AddComponent(typeof(FoodBarComponent));
        AddComponent(new InteractableInventoryComponent(){
            CarryLimit = 50,
            InventoryTemperature = TemperatureType.HOT
        });
        AddComponent(typeof(SecurityLockSocket));
        AddComponent(new InteractableLocationComponent());
        AddComponent(new CommercableLocationComponent());
        AddBuffer<ItemElement>();
    }
}