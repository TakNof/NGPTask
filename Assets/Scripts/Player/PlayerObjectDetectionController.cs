using UnityEngine;
using UnityEngine.Events;

public class PlayerObjectDetectionController : MonoBehaviour{
    public UnityEvent<Collider> OnTriggerEntered;
    public UnityEvent<Collider> OnTriggerExited;

    void OnTriggerEnter(Collider other){
        OnTriggerEntered?.Invoke(other);
    }

    void OnTriggerExit(Collider other){
        OnTriggerExited?.Invoke(other);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
