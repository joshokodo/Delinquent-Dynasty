using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ToiletGO : SeatGO {
    public GameObject stallDoorGO;

    private void LateUpdate(){
        if (SeatEntity == Entity.Null){
            return;
        }

        // var stall = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<InteractableLocationComponent>(Entity);
        //
        // if (stall.IsOccupied && !IsOccupied){
        //     SetOccupy(true);
        // } else if (!stall.IsOccupied && IsOccupied){
        //     SetOccupy(false);
        // }
    }

    public void SetOccupy(bool val){
        IsOccupied = val;
        if (stallDoorGO != null){
            stallDoorGO.transform.Rotate(val ? new Vector3(78, 0, 0) : new Vector3(-78, 0, 0));
        }
    }
}