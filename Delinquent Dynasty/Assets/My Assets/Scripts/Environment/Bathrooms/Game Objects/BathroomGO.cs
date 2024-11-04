using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public class BathroomGO : RoomGO {
    public List<ToiletGO> bathroomStalls;
    public bool isFemaleBathroom;
    public List<TrashCanGO> trashCans;
}