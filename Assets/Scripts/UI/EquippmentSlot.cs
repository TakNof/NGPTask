using UnityEngine;
using UnityEngine.EventSystems;

public class EquippmentSlot : MonoBehaviour, IDropHandler{
    [SerializeField] private string equipmentHand;

    [HideInInspector] public string EquipmentHand {get{return equipmentHand;}}

    public void OnDrop(PointerEventData eventData){
        if (eventData == null || eventData.pointerDrag == null)
            return;

        if (!eventData.pointerDrag.TryGetComponent<InventoryItem>(out var objectDropped))
            return;

        if(!objectDropped.itemData.isEquippable)
            return;

        if (transform.childCount == 1){
            objectDropped.parentAfterDrag = transform;
            InventoryManager.Instance.AddItemToEquipment(objectDropped.itemData, equipmentHand);
        }
    }
}
