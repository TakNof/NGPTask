using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler{
    [Header("UI")]
    [HideInInspector] public Transform parentAfterDrag;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;

    [Header("Gameplay")]
    [HideInInspector] public ItemData itemData;
    [HideInInspector] public int amount = 1;

    private void Start(){
        InitializeItem(itemData);
    }

    public void InitializeItem(ItemData newItemData){
        itemData = newItemData;
        iconImage.sprite = newItemData.sprite;
        RefreshCounter();
    }

    public void RefreshCounter(){
        bool shouldShow = amount > 1;
        countText.gameObject.SetActive(shouldShow);
        countText.text = amount.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData){
        parentAfterDrag = transform.parent;
            if (parentAfterDrag.TryGetComponent<EquippmentSlot>(out var equipmentSlot)){
            InventoryManager.Instance.RemoveItemFromEquipment(
                equipmentSlot.EquipmentHand,
                this
            );
        }

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        iconImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData){
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData){
        transform.SetParent(parentAfterDrag, false);
        transform.SetAsFirstSibling();

        var rectTransform = transform as RectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        iconImage.raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData){
        InventoryManager.Instance.CurrentHoveredItem = this;
    }

    public void OnPointerExit(PointerEventData eventData){
        InventoryManager.Instance.CurrentHoveredItem = null;
    }

    void Reset(){
        iconImage = GetComponent<Image>();
    }
}
