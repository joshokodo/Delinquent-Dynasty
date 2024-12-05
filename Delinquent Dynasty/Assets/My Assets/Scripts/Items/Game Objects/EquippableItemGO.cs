using System;
using UnityEngine;

public class EquippableItemGO : MonoBehaviour , IPooledObject {
    public event Action<GameObject> RecallEvent;
    public void OnActivation(){
        
    }

    public void Recall(){
        RecallEvent?.Invoke(gameObject);
    }
}