using Unity.Entities;
using UnityEngine;

public class TrendingPopupAuthoring : MonoBehaviour { }

public class TrendingPopupBaker : Baker<TrendingPopupAuthoring> {
    public override void Bake(TrendingPopupAuthoring authoring){
        AddComponentObject(new TrendingPopupComponent(){
            Popup = GameObject.FindObjectOfType<TrendingPopup>(true)
        });
    }
}