using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerInteractionController : MonoBehaviour{
    [Header("References")]
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private PlayerObjectDetectionController _detectionCtrl;

    [Header("Detection")]
    [SerializeField] private List<Collider> detectedObjects;

    [Header("Events")]
    public UnityEvent<Collider> OnInteractiveEntered => _detectionCtrl.OnTriggerEntered;
    public UnityEvent<Collider> OnInteractiveExited => _detectionCtrl.OnTriggerExited;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        _detectionCtrl.OnTriggerEntered.AddListener(AddDetectedToList);
        _detectionCtrl.OnTriggerExited.AddListener(DeleteDetectedFromList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DeleteDetectedFromList(Collider detectedObj){
        if(!detectedObjects.Contains(detectedObj)){
            Debug.Log("Detected Object wasn't in list");
            return;
        }

        detectedObjects.Remove(detectedObj);
    }

    private void AddDetectedToList(Collider detectedObj){
        if(detectedObjects.Contains(detectedObj)){
            Debug.Log("Detected Object was already in list");
            return;
        }

        detectedObjects.Add(detectedObj);
    }

    void Reset(){
        _input = GetComponent<PlayerInputController>();
    }
}
