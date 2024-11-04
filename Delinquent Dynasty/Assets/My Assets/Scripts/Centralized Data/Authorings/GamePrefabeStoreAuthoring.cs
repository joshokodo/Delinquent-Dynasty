using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class GamePrefabeStoreAuthoring : MonoBehaviour {
    public GameObject studentPrefab;
    public GameObject emptyPrefab;
    public GameObject vendingMachinePrefab;
    public GameObject lockerPrefab;
    public GameObject bathroomPrefab;
    public GameObject bathroomStallPrefab;
    public GameObject studentDeskPrefab;
    public GameObject classroomPrefab;
    public GameObject doorPrefab;
    public GameObject sinkPrefab;
    public GameObject bedPrefab;
    public GameObject dormPrefab;
    public GameObject buildingPrefab;
    public GameObject traitPrefab;
    public GameObject passiveEffectPrefab;
    public GameObject activeEffectPrefab;
    public GameObject fridgePrefab;
    public GameObject droppedItemPrefab;
    public GameObject foodBarPrefab;
    public GameObject cafeteriaSeatPrefab;
    public GameObject cafeteriaPrefab;
    public GameObject cafeteriaKitchenPrefab;
    public GameObject hallwayPrefab;
    public GameObject npcStatePrefab;
    public GameObject pathPrefab;
    public GameObject stovePrefab;
    public GameObject trashCanPrefab;
}

public class GamePrefabeStoreBaker : Baker<GamePrefabeStoreAuthoring> {
    public override void Bake(GamePrefabeStoreAuthoring authoring){
        AddComponent(new GamePrefabStore(){
            CharacterEntityPrefab = GetEntity(authoring.studentPrefab.GetComponent<CharacterAuthoring>()),
            EmptyPrefab = GetEntity(authoring.emptyPrefab),
            VendingMachinePrefab = GetEntity(authoring.vendingMachinePrefab),
            LockerPrefab = GetEntity(authoring.lockerPrefab),
            BathroomPrefab = GetEntity(authoring.bathroomPrefab),
            DeskPrefab = GetEntity(authoring.studentDeskPrefab),
            ToiletPrefab = GetEntity(authoring.bathroomStallPrefab),
            ClassroomPrefab = GetEntity(authoring.classroomPrefab),
            DoorPrefab = GetEntity(authoring.doorPrefab),
            SinkPrefab = GetEntity(authoring.sinkPrefab),
            DormPrefab = GetEntity(authoring.dormPrefab),
            BedPrefab = GetEntity(authoring.bedPrefab),
            BuildingPrefab = GetEntity(authoring.buildingPrefab),
            TraitPrefab = GetEntity(authoring.traitPrefab),
            PassiveEffectPrefab = GetEntity(authoring.passiveEffectPrefab),
            ActiveEffectPrefab = GetEntity(authoring.activeEffectPrefab),
            FridgePrefab = GetEntity(authoring.fridgePrefab),
            DroppedItemPrefab = GetEntity(authoring.droppedItemPrefab),
            FoodBarPrefab = GetEntity(authoring.foodBarPrefab),
            CafeteriaSeatPrefab = GetEntity(authoring.cafeteriaSeatPrefab),
            CafeteriaPrefab = GetEntity(authoring.cafeteriaPrefab),
            CafeteriaKitchenPrefab = GetEntity(authoring.cafeteriaKitchenPrefab),
            BuildingHallwayPrefab = GetEntity(authoring.hallwayPrefab),
            NpcStatePrefab = GetEntity(authoring.npcStatePrefab),
            PathPrefab = GetEntity(authoring.pathPrefab),
            StovePrefab = GetEntity(authoring.stovePrefab),
            TrashCanPrefab = GetEntity(authoring.trashCanPrefab),
        });
    }
}