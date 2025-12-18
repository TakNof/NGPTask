using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(PlayerCombatController))]
public class PlayerInteractionController : MonoBehaviour{
    [Header("References")]
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private PlayerObjectDetectionController _detectionCtrl;

    [Header("Detection")]
    [SerializeField] private List<InteractiveElement> detectedObjects;

    [Header("Events")]
    public UnityEvent<InteractiveElement> OnInteractiveEntered => _detectionCtrl.OnTriggerEntered;
    public UnityEvent<InteractiveElement> OnInteractiveExited => _detectionCtrl.OnTriggerExited;
    public UnityEvent<InteractiveElement> OnDetectedObjectsUpdated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        _detectionCtrl.OnTriggerEntered.AddListener(AddDetectedToList);
        _detectionCtrl.OnTriggerExited.AddListener(DeleteDetectedFromList);

        _input.interactAction.performed += (ctx) => AddDetectedObjectToInventory();
        _input.dropItemAction.performed += (ctx) => SpawnDroppedItem();
    }

    void Update(){
    }

    private void DeleteDetectedFromList(InteractiveElement detectedObj){
        if(!detectedObjects.Contains(detectedObj)){
            Debug.Log("Detected Object wasn't in list");
            return;
        }

        detectedObjects.Remove(detectedObj);
        detectedObj.SetNormal();
        if (detectedObjects.Count > 0)
            OnDetectedObjectsUpdated?.Invoke(detectedObjects[^1]);
        else
            OnDetectedObjectsUpdated?.Invoke(null);
    }

    private void AddDetectedToList(InteractiveElement detectedObj){
        if(detectedObjects.Contains(detectedObj)){
            Debug.Log("Detected Object was already in list");
            return;
        }

        detectedObjects.Add(detectedObj);
        detectedObj.SetHighlight();
        OnDetectedObjectsUpdated?.Invoke(detectedObjects[^1]);
    }

    private void AddDetectedObjectToInventory(){
        if(detectedObjects.Count == 0) return;
        
        var detectedInteractive = detectedObjects[0];

        bool canAdd = InventoryManager.Instance.AddItemToInventory(detectedInteractive.itemData);
        if(canAdd)
            OnInteractiveExited?.Invoke(detectedInteractive);
            Destroy(detectedInteractive.gameObject);
    }

    private void SpawnDroppedItem(){
        var itemToSpawn = InventoryManager.Instance.DropItemFromInventory();
        if(itemToSpawn == null) return;

        Instantiate(itemToSpawn, transform.position + Vector3.up * 2 + transform.forward * 2, Quaternion.identity);
    }

    void Reset(){
        _input = GetComponent<PlayerInputController>();
    }
}
