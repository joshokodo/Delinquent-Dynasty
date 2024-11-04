using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryLockButton : MonoBehaviour {
    public Image lockStatusImage;

    public void SetLock(bool isLocked){
        lockStatusImage.color = isLocked ? Color.green : Color.grey;
    }
}