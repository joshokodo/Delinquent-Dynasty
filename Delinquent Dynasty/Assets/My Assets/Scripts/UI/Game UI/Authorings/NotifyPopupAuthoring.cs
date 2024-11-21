using Unity.Entities;
using UnityEngine;

namespace My_Assets.Scripts.UI.Game_UI.Authorings {
    public class NotifyPopupAuthoring : MonoBehaviour { }

    public class NotifyPopupBaker : Baker<NotifyPopupAuthoring> {
        public override void Bake(NotifyPopupAuthoring authoring){
            AddComponentObject(new NotifyPopupComponent(){
                Popup = GameObject.FindObjectOfType<NotifyPopup>(true)
            });
        }
    }
}