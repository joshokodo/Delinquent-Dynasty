using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class TrashCanAuthoring : MonoBehaviour { }

public class TrashCanBaker : Baker<TrashCanAuthoring> {
    public override void Bake(TrashCanAuthoring authoring){
        AddComponent(new TrashCanComponent());
        AddComponent(new InteractableInventoryComponent(){
            AllowedItemProperties = new(){
                ItemPropertyType.TRASH_CONTAINER
            },
            CarryLimit = 1
        });
        AddComponent(new InteractableLocationComponent());
        AddBuffer<ItemElement>();
    }
}