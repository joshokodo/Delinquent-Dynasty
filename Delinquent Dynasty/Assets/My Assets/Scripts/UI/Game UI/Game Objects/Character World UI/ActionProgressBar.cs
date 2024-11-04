using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ActionProgressBar : MonoBehaviour {
    public Image barImage;
    public Image bgImage;
    public DynamicActionType DynamicActionType;
    public bool wasUpdated;

    public void SetBar(DynamicActionType dynamicActionType, Sprite sprite){
        gameObject.SetActive(true);
        barImage.sprite = sprite;
        bgImage.sprite = sprite;
        this.DynamicActionType = dynamicActionType;
    }

    public void ClearBar(){
        gameObject.SetActive(false);
        barImage.sprite = null;
        bgImage.sprite = null;
        DynamicActionType = default;
    }
}