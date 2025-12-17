using UnityEngine;
using UnityEngine.Tilemaps;
public enum ItemType{
    Weapon,
    Material,
    Potion
}

public enum ActionType{
    Attack,
    Build,
    Drink
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject{
    
    [Header("Gameplay")]
    public ItemType type;
    public ActionType action;

    [Header("UI")]
    public bool isStackable = true;

    [Header("Both")]
    public Sprite sprite;
    

    
}
