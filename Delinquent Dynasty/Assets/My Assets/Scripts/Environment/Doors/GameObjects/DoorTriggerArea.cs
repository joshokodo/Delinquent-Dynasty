using System;
using UnityEngine;

public class DoorTriggerArea : MonoBehaviour {
    public int CharacterCount{ get; private set; }

    private void OnTriggerEnter(Collider other){
        if ("Student".Equals(other.tag) || "Faculty".Equals(other.tag)){
            CharacterCount++;
        }
    }

    private void OnTriggerExit(Collider other){
        if ("Student".Equals(other.tag) || "Faculty".Equals(other.tag)){
            CharacterCount--;
        }
    }
}