using Unity.Entities;
using UnityEngine;

public class CafeteriaAuthoring : MonoBehaviour { }

public class CafeteriaBaker : Baker<CafeteriaAuthoring> {
    public override void Bake(CafeteriaAuthoring authoring){
        AddComponent(new RoomComponent());
    }
}