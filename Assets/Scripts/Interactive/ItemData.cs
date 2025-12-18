using UnityEngine;
public enum ItemType{
    Weapon,
    Material,
    Potion
}

public enum ActionType{
    Attack,
    Upgrade,
    Drink
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject{
    
    [Header("Gameplay")]
    public GameObject prefab;
    public bool isEquippable = false;

    [Header("UI")]
    public bool isStackable = true;

    [Header("Both")]
    public ItemType type;
    public ActionType action;
    public Sprite sprite;
    public string description;
}
