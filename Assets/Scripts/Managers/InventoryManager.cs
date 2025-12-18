using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour{
    public enum InventorySection{
        General,
        LeftHand,
        RightHand,
        ObjectSlots
    }

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
        Debug.Log($"Preparing to add {itemData.name} to inventory");

        Debug.Log("Checking Stackable");
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

    public bool AddItemToInventory(ItemData itemData, InventorySlot slot, int amout){

        var existingItem = slot.GetComponentInChildren<InventoryItem>();
        if(existingItem != null && slot.transform.childCount == 2){

            if (existingItem.itemData == itemData && itemData.isStackable){
                int space = maxStackAmount - existingItem.amount;
                if (space <= 0) return false;

                int toAdd = Mathf.Min(space, amout);
                existingItem.amount += toAdd;
                existingItem.RefreshCounter();
                amout -= toAdd;

                return amout == 0;
            }
            return false;
        }

        var newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        var newInvItem = newItemGo.GetComponent<InventoryItem>();
        newInvItem.InitializeItem(itemData);

        if(itemData.isStackable){
            int toPlace = Mathf.Min(maxStackAmount, amout);
            newInvItem.amount = toPlace;
            newInvItem.RefreshCounter();
            amout -= toPlace;
            return amout == 0;
        }else{
            newInvItem.amount = 1;
            newInvItem.RefreshCounter();
            amout -= 1;
            return amout == 0;
        }
    }

    public bool AddItemToObjects(ItemData itemData){
        foreach(var slot in ObjectsToUseSlots){
            bool canAddItem = StackItemToSlot(itemData, slot);
            if(!canAddItem) continue;
            return true;
        }

        foreach (var slot in ObjectsToUseSlots){
            if(slot.transform.childCount > 1) continue;
            SpawnNewItem(itemData, slot);
            return true;
        } 

        return false;
    }

    public bool AddItemToObjects(ItemData itemData, InventorySlot slot){

        bool canAddItem = StackItemToSlot(itemData, slot);
        if(canAddItem) return true;

        if(slot.transform.childCount > 1) return false;
        SpawnNewItem(itemData, slot);
        return true;
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

    public bool AddItemToEquipment(ItemData itemData, string handToEquip, EquippmentSlot slot, int amount){
        if(!itemData.isEquippable || slot == null) return false;
        if(amount <= 0) return false;

        for(int i = 0; i < amount; i++){
            bool canEquip;
            if(handToEquip == "left" || handToEquip == "right"){
                canEquip = CheckEquippable(itemData, slot, handToEquip);
            }else{
                Debug.LogError("Error, no hand choosen correctly");
                return false;
            }

            if(!canEquip) return false;
        }
        SpawnNewItem(itemData, slot);
        return true;
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

    private bool CheckEquippable(ItemData itemData, EquippmentSlot slot, string handToEquip){
        if (slot.transform.childCount != 1) return false;
        _plCombCtrl.EquipItemToHand(handToEquip, itemData);
        return true;
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

            itemInSlot.amount ++;
            itemInSlot.RefreshCounter();
            return true;
    }

    public bool StackItemToSlot(InventoryItem inventoryItem, InventorySlot slot){
        var itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null ||
                slot.transform.childCount != 2 ||
                itemInSlot.itemData != inventoryItem.itemData ||
                !itemInSlot.itemData.isStackable ||
                itemInSlot.amount >= maxStackAmount ||
                itemInSlot.amount + inventoryItem.amount >= maxStackAmount)
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

    public InventorySaveData GetSaveData(){
        InventorySaveData data = new();

        SaveSection(data, InventorySection.General, inventorySlots);
        SaveSection(data, InventorySection.LeftHand, LeftHandEquipmentSlots);
        SaveSection(data, InventorySection.RightHand, RightHandEquipmentSlots);
        SaveSection(data, InventorySection.ObjectSlots, ObjectsToUseSlots);

        return data;
    }

    private void SaveSection(
        InventorySaveData data,
        InventorySection section,
        InventorySlot[] slots
    ){
        for (int i = 0; i < slots.Length; i++){
            var slot = slots[i];

            if (slot.transform.childCount <= 1)
                continue;

            var item = slot.GetComponentInChildren<InventoryItem>();

            data.items.Add(new InventoryItemSaveData{
                section = section,
                slotIndex = i,
                itemId = item.itemData.name,
                amount = item.amount
            });
        }
    }

    private void SaveSection(
        InventorySaveData data,
        InventorySection section,
        EquippmentSlot[] slots
    ){
        for (int i = 0; i < slots.Length; i++){
            var slot = slots[i];

            if (slot.transform.childCount <= 1)
                continue;

            var item = slot.GetComponentInChildren<InventoryItem>();

            data.items.Add(new InventoryItemSaveData{
                section = section,
                slotIndex = i,
                itemId = item.itemData.name,
                amount = item.amount
            });
        }
    }

    private void ClearAllSections(){
        ClearSlots(inventorySlots);
        ClearSlots(LeftHandEquipmentSlots);
        ClearSlots(RightHandEquipmentSlots);
        ClearSlots(ObjectsToUseSlots);
        _plCombCtrl.DeleteHandsEquipment();
    }

    private void ClearSlots(InventorySlot[] slots){
        foreach (var slot in slots){
            if (slot.transform.childCount > 1){
                Destroy(slot.GetComponentInChildren<InventoryItem>().gameObject);
            }
        }
    }

    private void ClearSlots(EquippmentSlot[] slots){
        foreach (var slot in slots){
            if (slot.transform.childCount > 1){
                Destroy(slot.GetComponentInChildren<InventoryItem>().gameObject);
            }
        }
    }

    public InventorySlot[] GetInventorySlotsBySection(InventorySection section){
        return section switch{
            InventorySection.General => inventorySlots,
            InventorySection.ObjectSlots => ObjectsToUseSlots,
            _ => null
        };
    }

    public EquippmentSlot[] GetEquipmentSlotsBySection(InventorySection section){
        return section switch{
            InventorySection.LeftHand => LeftHandEquipmentSlots,
            InventorySection.RightHand => RightHandEquipmentSlots,
            _ => null
        };
    }

    public void LoadInventory(InventorySaveData data){
        if (data == null) return;

        ClearAllSections();

        foreach (var savedItem in data.items){
            if (savedItem.section == InventorySection.General || savedItem.section == InventorySection.ObjectSlots){
                var slots = GetInventorySlotsBySection(savedItem.section);
                if (slots == null || savedItem.slotIndex >= slots.Length)
                    continue;

                ItemData itemSO = ItemDatabase.Instance.GetItem(savedItem.itemId);
                if (itemSO == null)
                    continue;

                AddItemToInventory(itemSO, slots[savedItem.slotIndex], savedItem.amount);
            } else { // LeftHand or RightHand
                var slots = GetEquipmentSlotsBySection(savedItem.section);
                if (slots == null || savedItem.slotIndex >= slots.Length)
                    continue;

                ItemData itemSO = ItemDatabase.Instance.GetItem(savedItem.itemId);
                if (itemSO == null)
                    continue;

                string handString = savedItem.section == InventorySection.LeftHand ? "left" : "right";

                AddItemToEquipment(itemSO, handString, slots[savedItem.slotIndex], savedItem.amount);
            }
        }
    }

    private void SpawnNewItem(ItemData itemData, InventorySlot slot){
        var newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        var invetoryItem = newItemGo.GetComponent<InventoryItem>();
        invetoryItem.InitializeItem(itemData);
    }

    private void SpawnNewItem(ItemData itemData, EquippmentSlot slot){
        var newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        var invetoryItem = newItemGo.GetComponent<InventoryItem>();
        invetoryItem.InitializeItem(itemData);
    }

    [System.Serializable]
    public class InventoryItemSaveData{
        public InventorySection section;
        public int slotIndex;
        public string itemId;
        public int amount;
    }

    [System.Serializable]
    public class InventorySaveData{
        public List<InventoryItemSaveData> items = new();
    }
}