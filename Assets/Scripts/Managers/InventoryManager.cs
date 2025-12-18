using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour{
    public static InventoryManager Instance {get; private set;}
    [Header("References")]
    [SerializeField] private PlayerCombatController _plCombCtrl;

    [Header("Prefabs")]
    [SerializeField] private GameObject inventoryItemPrefab;

    [Header("Settings")]
    [SerializeField] private int maxStackAmount = 12;
    [HideInInspector] public int MaxStackAmount {get{return maxStackAmount;}}

    [Header("Inventory Slots")]
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] EquippmentSlot[] LeftHandEquipmentSlots;
    [SerializeField] EquippmentSlot[] RightHandEquipmentSlots;
    [SerializeField] InventorySlot[] ObjectsToUseSlots;

    [Header("Details Panel References")]
    [SerializeField] private Image itemImagePrev;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemType;
    [SerializeField] private TMP_Text itemFunction;
    [SerializeField] private TMP_Text itemDescription;


    [Header("Data")]
    private InventoryItem currentHoveredItem;
    public InventoryItem CurrentHoveredItem
    {
        get{return currentHoveredItem;}
        set{
            if(value == currentHoveredItem) return;

            currentHoveredItem = value;
            if(currentHoveredItem == null){
                ClearDetailsPanel();
            }else{
                UpdateDetailsPanel();
            }
        }
    }
    

    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(this);
        }else{
            Instance = this;
            DontDestroyOnLoad(Instance);
        }

        ClearDetailsPanel();

        //I know this is not the better practice, but i'm racing against the clock haha
        //Actually this code would break if there is a change of scene...
        var playerGo = GameObject.FindWithTag("Player");
        _plCombCtrl = playerGo.GetComponent<PlayerCombatController>();
    }

    public bool AddItemToInventory(ItemData itemData){

        //Check stackable
        foreach(var slot in inventorySlots){
            bool canAddItem = StackItemToSlot(itemData, slot);
            if(!canAddItem) continue;
            return true;
        }

        //Check available slot
        foreach (var slot in inventorySlots){
            if(slot.transform.childCount > 1) continue;
            SpawnNewItem(itemData, slot);
            return true;
        } 

        return false;
    }

    public bool AddItemToEquipment(ItemData itemData, string handToEquip){
        if(!itemData.isEquippable) return false;

        bool canEquip;
        if(handToEquip == "left"){
            canEquip = CheckEquippable(itemData, LeftHandEquipmentSlots, handToEquip);
        }else if(handToEquip == "right"){
            canEquip = CheckEquippable(itemData, RightHandEquipmentSlots, handToEquip);
        }else{
            Debug.LogError("Error, no hand choosen correctly");
            return false;
        }

        if(canEquip) return true;

        return false;
    }

    public bool RemoveItemFromEquipment(string handToUnequip, InventoryItem inventoryItem){
        bool canUnequip = _plCombCtrl.UnequipItemFromHand(handToUnequip, inventoryItem);
        return canUnequip;
    }

    private bool CheckEquippable(ItemData itemData, EquippmentSlot[] equippmentSlots, string handToEquip){
        foreach(var slot in equippmentSlots){
            if (slot.transform.childCount != 1) continue;
            _plCombCtrl.EquipItemToHand(handToEquip, itemData);
            return true;
        }

        return false;
    }

    public bool StackItemToSlot(ItemData itemData, InventorySlot slot){
        var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null ||
                slot.transform.childCount != 2 ||
                itemInSlot.itemData != itemData ||
                !itemInSlot.itemData.isStackable ||
                itemInSlot.amount >= maxStackAmount)
            {
                return false;
            }

            itemInSlot.amount += itemInSlot.amount;
            itemInSlot.RefreshCounter();
            return true;
    }

    public bool StackItemToSlot(InventoryItem inventoryItem, InventorySlot slot){
        var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null ||
                slot.transform.childCount != 2 ||
                itemInSlot.itemData != inventoryItem.itemData ||
                !itemInSlot.itemData.isStackable ||
                itemInSlot.amount >= maxStackAmount)
            {
                return false;
            }

            itemInSlot.amount += inventoryItem.amount;
            itemInSlot.RefreshCounter();
            return true;
    }

    public GameObject DropItemFromInventory(){
        if(currentHoveredItem == null) return null;
        currentHoveredItem.transform.parent.TryGetComponent<EquippmentSlot>(out var equippmentSlot);

        currentHoveredItem.amount--;
        currentHoveredItem.RefreshCounter();

        if(currentHoveredItem.amount == 0){
            if(equippmentSlot != null){
                _plCombCtrl.UnequipItemFromHand(equippmentSlot.EquipmentHand, currentHoveredItem);
            }
            Destroy(currentHoveredItem.gameObject);

        }
        ClearDetailsPanel();
        return currentHoveredItem.itemData.prefab;
    }

    private void UpdateDetailsPanel(){
        var itemData = currentHoveredItem.itemData;
        itemImagePrev.sprite = itemData.sprite;

        itemName.text = itemData.name;
        itemType.text = "Type: " + itemData.type.ToString();
        itemFunction.text = "Function: " + itemData.action.ToString();
        itemDescription.text = itemData.description;

        itemImagePrev.gameObject.SetActive(true);
        itemName.gameObject.SetActive(true);
        itemType.gameObject.SetActive(true);
        itemFunction.gameObject.SetActive(true);
        itemDescription.gameObject.SetActive(true);
    }

    private void ClearDetailsPanel(){
        itemImagePrev.sprite = null;

        itemName.text = null;
        itemType.text = null;
        itemFunction.text = null;
        itemDescription.text = null;

        itemImagePrev.gameObject.SetActive(false);
        itemName.gameObject.SetActive(false);
        itemType.gameObject.SetActive(false);
        itemFunction.gameObject.SetActive(false);
        itemDescription.gameObject.SetActive(false);
    }

    private void SpawnNewItem(ItemData itemData, InventorySlot slot){
        var newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        var invetoryItem = newItemGo.GetComponent<InventoryItem>();
        invetoryItem.InitializeItem(itemData);
    }
}
