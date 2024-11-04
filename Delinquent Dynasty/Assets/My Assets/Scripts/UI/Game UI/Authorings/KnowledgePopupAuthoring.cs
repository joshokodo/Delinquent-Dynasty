using Unity.Entities;
using UnityEngine;

public class KnowledgePopupAuthoring : MonoBehaviour { }

public class KnowledgePopupBaker : Baker<KnowledgePopupAuthoring> {
    public override void Bake(KnowledgePopupAuthoring authoring){
        AddComponentObject(new KnowledgePopupComponent(){
            Popup = GameObject.FindObjectOfType<KnowledgePopup>(true)
        });
    }
}