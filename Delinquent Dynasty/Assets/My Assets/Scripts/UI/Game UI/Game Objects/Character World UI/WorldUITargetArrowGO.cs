using System;
using Arrow;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WorldUITargetArrowGO : MonoBehaviour, IPooledObject {
    public event Action<GameObject> RecallEvent;
    public AnimatedArrowRenderer Arrow;
    public Entity target;

    private void Awake(){
        Arrow = gameObject.GetComponent<AnimatedArrowRenderer>();
    }

    public void OnActivation(){ }

    public void SetArrow(Entity target){
        this.target = target;
    }

    public void Recall(){
        target = Entity.Null;
        RecallEvent.Invoke(gameObject);
    }
}