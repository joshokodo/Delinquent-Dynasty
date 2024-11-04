using Unity.Entities;
using UnityEngine;

public class ActionsPopupAuthoring : MonoBehaviour { }

public class PlansPopupBaker : Baker<ActionsPopupAuthoring> {
    public override void Bake(ActionsPopupAuthoring authoring){
        AddComponentObject(new ActionsPopupComponent(){
            Popup = GameObject.FindObjectOfType<ActionsPopup>(true)
        });
    }
}