using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class DormRoomGO : RoomGO {
    public bool isFemaleRoom;
    public BedGO bed1;
    public BedGO bed2;
    [FormerlySerializedAs("door")] public DoorGO mainDoor;
}