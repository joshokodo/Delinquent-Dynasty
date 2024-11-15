using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class DoorGO : MonoBehaviour {
    public string alias;
    public Transform LookPoint;
    public Entity DoorEntity;
    public ItemSpawnDTO lockItem;

    public DoorTriggerArea frontOfDoor;
    public DoorTriggerArea backOfDoor;
    public NavMeshObstacle navObstacle;
    public List<DoorOpener> doorOpeners;

    private void Awake(){
        navObstacle.enabled = false;
        navObstacle.carving = false;
        SetDoorOpeners(false);
    }

    private void LateUpdate(){
        if (DoorEntity == Entity.Null){
            return;
        }

        // var em = World.DefaultGameObjectInjectionWorld.EntityManager;
        //
        // var lockSocket = em.GetComponentData<LockItemSocket>(DoorEntity);
        // if (lockSocket.LockEntity != Entity.Null){
        //     var lockComponent =
        //         em.GetComponentData<ItemSecurityLockComponent>(
        //             lockSocket.LockEntity);
        //     if (lockComponent.IsLocked && !navObstacle.enabled){
        //         navObstacle.enabled = true;
        //         navObstacle.carving = true;
        //         SetDoorOpeners(true);
        //     } else if (!lockComponent.IsLocked && navObstacle.enabled){
        //         navObstacle.enabled = false;
        //         navObstacle.carving = false;
        //         SetDoorOpeners(false);
        //     }
        // } else if (navObstacle.enabled){
        //     navObstacle.carving = false;
        //     navObstacle.enabled = false;
        //     SetDoorOpeners(false);
        // }
    }

    private void SetDoorOpeners(bool val){
        doorOpeners.ForEach(d => d.IsOpen = val);
    }
}