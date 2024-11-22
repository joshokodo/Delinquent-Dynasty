using Unity.Entities;
using UnityEngine;

public class RequestPopupAuthoring : MonoBehaviour { }

public class RequestPopupBaker : Baker<RequestPopupAuthoring> {
    public override void Bake(RequestPopupAuthoring authoring){
        AddComponentObject(new RequestPopupComponent(){
            Popup = GameObject.FindObjectOfType<RequestPopup>(true)
        });
    }
}