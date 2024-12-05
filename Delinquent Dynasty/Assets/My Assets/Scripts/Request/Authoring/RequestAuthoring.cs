  using Unity.Entities;
  using UnityEngine;

  public class RequestAuthoring : MonoBehaviour {
      
  }
  
  public class RequestBaker : Baker<RequestAuthoring> {
    public override void Bake(RequestAuthoring authoring){
        AddBuffer<RequestEntityElement>();
    }
}