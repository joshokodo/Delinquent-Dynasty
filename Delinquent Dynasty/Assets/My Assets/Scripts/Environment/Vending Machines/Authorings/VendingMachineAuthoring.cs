using Unity.Entities;
using UnityEngine;

public class VendingMachineAuthoring : MonoBehaviour { }

public class VendingMachineBaker : Baker<VendingMachineAuthoring> {
    public override void Bake(VendingMachineAuthoring authoring){
        AddComponent(new VendingMachineTag());
        AddComponent(new InteractableInventoryComponent(){
            CarryLimit = 100,
            InventoryTempurature = TemperatureType.MID
        });
        AddComponent(new InteractableLocationComponent());
        AddComponent(typeof(SecurityLockSocket));
        AddComponent(new CommercableLocationComponent(){
            IsSelling = true
        });
        AddBuffer<ItemElement>();
    }
}