using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RoomGO : MonoBehaviour {
    public string roomName;
    public List<DoorGO> doors;
    public int floorNumber;
    public Entity RoomEntity;
    public AreaGO area;
}