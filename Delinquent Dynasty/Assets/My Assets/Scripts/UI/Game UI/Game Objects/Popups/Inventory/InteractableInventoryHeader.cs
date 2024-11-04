using UnityEngine;
using UnityEngine.EventSystems;

public class InteractableInventoryHeader : MonoBehaviour, IPointerClickHandler {
    public ListViewInventoryElement InventoryElement;

    public void OnPointerClick(PointerEventData eventData){
        if (eventData.button == PointerEventData.InputButton.Right){
            PlayerInputs.Instance.SelectedEntity = InventoryElement.inventoryEntity;
            PlayerInputs.Instance.inventorySelectedForQuickActions = true;
            PlayerInputs.Instance.entityTargets.Add(new TargetElement(){
                Data = new TargetData(){
                    TargetEntity = InventoryElement.inventoryEntity,
                    TargetType = TargetType.TARGET_INVENTORY,
                }
            });
        }
    }
}