using Unity.Entities;
using UnityEngine;

public class InventoryPopupAuthoring : MonoBehaviour { }

public class InventoryPopupBaker : Baker<InventoryPopupAuthoring> {
    public override void Bake(InventoryPopupAuthoring authoring){
        AddComponentObject(new InventoryPopupComponent(){
            Popup = GameObject.FindObjectOfType<InventoryPopup>(true)
        });
    }
}