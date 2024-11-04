using System;
using System.Collections.Generic;
using TMPro;
using UIWidgets;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class InteractablePopup : Popup {
    public TextMeshProUGUI Title;
    public Image InteractionImage;
    public InteractableLockIcon lockIcon;
    public event Action OnCloseOverride;
    public TextMeshProUGUI QueueMessage;
    public Entity InteractableEntity;

    public void SetDoor(Entity doorEntity, int positionInQueue, bool isLocked){
        Title.text = "Door";
        InteractableEntity = doorEntity;
        if (positionInQueue > 0){
            QueueMessage.gameObject.SetActive(true);
            lockIcon.gameObject.SetActive(false);
            QueueMessage.text = "IN QUEUE \n" + (positionInQueue + 1);
        }
        else{
            lockIcon.gameObject.SetActive(true);
            QueueMessage.gameObject.SetActive(false);
            lockIcon.SetLockedStatus(isLocked);
            lockIcon.InteractableEntity = doorEntity;
        }
    }

    public void SetToilet(Entity entity, int positionInQueue){
        SetCommon("Toilet", entity, positionInQueue);
    }

    public void SetSink(Entity entity, int positionInQueue){
        SetCommon("Sink", entity, positionInQueue);
    }

    public void SetBed(Entity entity, int positionInQueue){
        SetCommon("Bed", entity, positionInQueue);
    }

    private void SetCommon(string title, Entity entity, int positionInQueue){
        Title.text = title;
        InteractableEntity = entity;
        lockIcon.gameObject.SetActive(false);

        if (positionInQueue > 0){
            QueueMessage.gameObject.SetActive(true);
            QueueMessage.text = "IN QUEUE \n" + (positionInQueue + 1);
        }
        else{
            QueueMessage.gameObject.SetActive(false);
        }
    }

    public void ClosePanel(){
        OnCloseOverride?.Invoke();
    }
}