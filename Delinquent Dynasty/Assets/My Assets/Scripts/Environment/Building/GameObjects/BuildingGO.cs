using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGO : MonoBehaviour {
    public string buildingName;
    public List<GameObject> floors;
    public List<BathroomGO> bathrooms = new();
    public List<DormRoomGO> dormRooms = new();
    public List<ClassroomGO> classrooms = new();
    public List<BuildingHallwayGO> hallways = new();
    public List<CafeteriaGO> cafeterias = new();

    public List<DoorGO> doors = new();

    private void Awake(){
        floors.ForEach(f => {
            bathrooms.AddRange(f.GetComponentsInChildren<BathroomGO>());
            dormRooms.AddRange(f.GetComponentsInChildren<DormRoomGO>());
            classrooms.AddRange(f.GetComponentsInChildren<ClassroomGO>());
            hallways.AddRange(f.GetComponentsInChildren<BuildingHallwayGO>());
            cafeterias.AddRange(f.GetComponentsInChildren<CafeteriaGO>());
        });
    }

    private const int DEFAULT_LAYER = 0;
    private const int WALL_LAYER = 6;
    private const int CHARACTER_LAYER = 7;
    private const int GROUND_LAYER = 8;
    private const int INTERACTABLE_LAYER = 9;

    public void ShowFloor(int floor){
        for (int i = 0; i < floors.Count; i++){
            var nextTransform = floors[i].transform;
            if (i <= floor - 1){
                ShowFloor(nextTransform);
            }
            else{
                HideFloor(nextTransform);
            }
        }
    }

    private void HideFloor(Transform parent){
        foreach (Renderer r in parent.GetComponentsInChildren<Renderer>()){
            r.enabled = false;
        }

        parent.gameObject.layer = DEFAULT_LAYER;

        foreach (Transform child in parent.transform){
            if (child.childCount > 0){
                HideFloor(child);
            }

            child.gameObject.layer = DEFAULT_LAYER;
        }
    }

    private void ShowFloor(Transform parent){
        foreach (Renderer r in parent.GetComponentsInChildren<Renderer>()){
            r.enabled = true;
        }

        switch (parent.gameObject.tag){
            case "Interactable":
            case "Seat":
            case "Locker":
            case "Sink":
            case "Toilet":
            case "Desk":
            case "Door":
            case "Bed":
                parent.gameObject.layer = INTERACTABLE_LAYER;
                break;

            case "Wall":
                parent.gameObject.layer = WALL_LAYER;
                break;

            case "Ground":
            case "Stairs":
                parent.gameObject.layer = GROUND_LAYER;
                break;
        }

        foreach (Transform child in parent.transform){
            if (child.childCount > 0){
                ShowFloor(child);
            }

            switch (child.gameObject.tag){
                case "Interactable":
                case "Seat":
                case "Locker":
                case "Sink":
                case "Toilet":
                case "Desk":
                case "Door":
                    child.gameObject.layer = INTERACTABLE_LAYER;
                    break;

                case "Wall":
                    child.gameObject.layer = WALL_LAYER;
                    break;

                case "Ground":
                case "Stairs":
                    child.gameObject.layer = GROUND_LAYER;
                    break;
            }
        }
    }
}