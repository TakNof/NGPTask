using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler{
    public void OnDrop(PointerEventData eventData){
        if(transform.childCount != 1) return;
        var objectDropped = eventData.pointerDrag.GetComponent<InventoryItem>();
        objectDropped.parentAfterDrag = transform;
    }
}
