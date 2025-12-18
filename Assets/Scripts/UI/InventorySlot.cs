using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler{
    public void OnDrop(PointerEventData eventData){
        if (eventData == null || eventData.pointerDrag == null)
            return;

        if (!eventData.pointerDrag.TryGetComponent<InventoryItem>(out var objectDropped))
            return;

        if (transform.childCount == 1){
            objectDropped.parentAfterDrag = transform;
        }else{
            bool canAddItem = InventoryManager.Instance.StackItemToSlot(objectDropped, this);
            if(canAddItem)
                Destroy(objectDropped.gameObject);
        }
    }
}
