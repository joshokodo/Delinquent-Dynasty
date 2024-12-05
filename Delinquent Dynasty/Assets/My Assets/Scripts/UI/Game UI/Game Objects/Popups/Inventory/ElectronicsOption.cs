using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElectronicsOption : MonoBehaviour, IPointerClickHandler {
    public Entity ItemEntity;
    public ElectronicsOptionType ElectronicsOptionType;
    
    public TextMeshProUGUI textCount;
    public GameObject textCountBackground;
    
    
    public void OnPointerClick(PointerEventData eventData){
        if (eventData.button == PointerEventData.InputButton.Right){
            PlayerInputs.Instance.electronicsOptionSelectedForQuickActions = true;
            PlayerInputs.Instance.selectedElectronicsType = ElectronicsOptionType;
            PlayerInputs.Instance.SelectedEntity = ItemEntity;

            PlayerInputs.Instance.entityTargets.Add(new TargetElement(){
                Data = new TargetData(){
                    TargetEntity = ItemEntity,
                    TargetType = TargetType.TARGET_ITEM,
                }
            });
        }
    }

    public void SetCount(int count){
        if (count <= 0){
            textCount.gameObject.SetActive(false);
            textCountBackground.SetActive(false);
        } else if (count <= 99){
            textCount.gameObject.SetActive(true);
            textCount.text = count.ToString();
            textCountBackground.SetActive(true);
        }
        else{
            textCount.gameObject.SetActive(true);
            textCount.text = "+99";
            textCountBackground.SetActive(true);
        }
    }
}

public enum ElectronicsOptionType {
    NONE,
    SETTINGS,
    CALL,
    TEXT,
    CONTACTS,
    APPS,
    VIDEO_MEDIA,
    TEXT_MEDIA,
    SOUND_MEDIA,
    PHOTO_MEDIA,
    GAMES,
}