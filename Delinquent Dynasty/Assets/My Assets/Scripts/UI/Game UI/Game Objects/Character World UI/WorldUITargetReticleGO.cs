using System;
using Unity.Entities;
using UnityEngine;

public class WorldUITargetReticleGO : MonoBehaviour, IPooledObject {
    public RectTransform rectTransform;
    public event Action<GameObject> RecallEvent;

    private void Awake(){
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnActivation(){ }

    public void Recall(){
        RecallEvent.Invoke(gameObject);
    }
}