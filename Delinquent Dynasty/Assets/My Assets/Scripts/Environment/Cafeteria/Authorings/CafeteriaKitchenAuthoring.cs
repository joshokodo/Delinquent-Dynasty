using Unity.Entities;
using UnityEngine;

public class CafeteriaKitchenAuthoring : MonoBehaviour { }

public class CafeteriaKitchenBaker : Baker<CafeteriaKitchenAuthoring> {
    public override void Bake(CafeteriaKitchenAuthoring authoring){
        AddComponent(new RoomComponent());
    }
}