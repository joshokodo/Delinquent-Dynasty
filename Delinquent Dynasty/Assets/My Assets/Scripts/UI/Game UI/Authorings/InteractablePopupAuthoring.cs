using Unity.Entities;
using UnityEngine;

public class InteractablePopupAuthoring : MonoBehaviour { }

public class InteractablePopupBaker : Baker<InteractablePopupAuthoring> {
    public override void Bake(InteractablePopupAuthoring authoring){
        AddComponentObject(new InteractablePopupComponent(){
            Popup = GameObject.FindObjectOfType<InteractablePopup>(true)
        });
    }
}