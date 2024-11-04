using Unity.Entities;
using UnityEngine;

public class RelationshipPopupAuthoring : MonoBehaviour { }

public class RelationshipPopupBaker : Baker<RelationshipPopupAuthoring> {
    public override void Bake(RelationshipPopupAuthoring authoring){
        AddComponentObject(new CharacterBioPopupComponent(){
            Popup = GameObject.FindObjectOfType<CharacterBioPopup>(true)
        });
    }
}