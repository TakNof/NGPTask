using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    [Header("Todos los ItemData del juego")]
    [SerializeField] private List<ItemData> allItems;

    private Dictionary<string, ItemData> itemLookup;

    private void Awake(){
        // Singleton simple
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }

        Instance = this;

        BuildLookup();
    }

    private void BuildLookup(){
        itemLookup = new Dictionary<string, ItemData>();

        foreach (var item in allItems){
            if (item == null) continue;

            if (string.IsNullOrEmpty(item.name)){
                Debug.LogWarning(
                    $"ItemData '{item.name}' no tiene itemId asignado",
                    item
                );
                continue;
            }

            if (itemLookup.ContainsKey(item.name)){
                Debug.LogWarning(
                    $"ItemID duplicado '{item.name}'",
                    item
                );
                continue;
            }

            itemLookup.Add(item.name, item);
        }
    }

    /// <summary>
    /// Devuelve el ItemData asociado a un ID.
    /// Retorna null si no existe.
    /// </summary>
    public ItemData GetItem(string itemId){
        if (string.IsNullOrEmpty(itemId))
            return null;

        itemLookup.TryGetValue(itemId, out var item);
        return item;
    }
}
