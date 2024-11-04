using Unity.Entities;
using UnityEngine;

public class CafeteriaSeatAuthoring : MonoBehaviour { }

public class CafeteriaSeatBaker : Baker<CafeteriaSeatAuthoring> {
    public override void Bake(CafeteriaSeatAuthoring authoring){
        AddComponent(new SeatTag());
        AddComponent(new CafeteriaSeatComponent());
        AddComponent(new InteractableLocationComponent());
    }
}