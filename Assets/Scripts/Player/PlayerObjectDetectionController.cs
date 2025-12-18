using UnityEngine;
using UnityEngine.Events;

public class PlayerObjectDetectionController : MonoBehaviour{
    public UnityEvent<InteractiveElement> OnTriggerEntered;
    public UnityEvent<InteractiveElement> OnTriggerExited;

    void OnTriggerEnter(Collider other){
        var ie = other.GetComponent<InteractiveElement>();
        OnTriggerEntered?.Invoke(ie);
    }

    void OnTriggerExit(Collider other){
        var ie = other.GetComponent<InteractiveElement>();
        OnTriggerExited?.Invoke(ie);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
