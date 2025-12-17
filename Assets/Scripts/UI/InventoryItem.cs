using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{
    [HideInInspector] public Transform parentAfterDrag;
    [SerializeField] private Image iconImage;

    [HideInInspector] public ItemData itemData;

    private void Start(){
        InitializeItem(itemData);
    }

    public void InitializeItem(ItemData newItemData){
        itemData = newItemData;
        iconImage.sprite = newItemData.sprite;
    }

    public void OnBeginDrag(PointerEventData eventData){
        Debug.Log("Drag start");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        iconImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData){
        Debug.Log("Dragging");
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData){
        Debug.Log("Drag stop");
        transform.SetParent(parentAfterDrag, false);
        transform.SetSiblingIndex(1);

        var rectTransform = transform as RectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        iconImage.raycastTarget = true;
    }

    void Reset(){
        iconImage = GetComponent<Image>();
    }
}
