using Unity.Entities;
using UnityEngine;

public class SinkAuthoring : MonoBehaviour { }

public class SinkBaker : Baker<SinkAuthoring> {
    public override void Bake(SinkAuthoring authoring){
        AddComponent(typeof(InteractableLocationComponent));
        AddComponent(typeof(SinkTag));
    }
}