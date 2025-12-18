using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerCombatController : MonoBehaviour{
    [Header("References")]
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private Transform leftHandBone;
    [HideInInspector] public Transform LeftHandBone {get{return leftHandBone;}}
    [SerializeField] private Transform rightHandBone;
    [HideInInspector] public Transform RightHandBone {get{return rightHandBone;}}

    [Header("Data")]
    [SerializeField] private Rigidbody rightHandEquippedObject;
    [SerializeField] private Rigidbody leftHandEquippedObject;

    private readonly string EquippedLayer = "Equipped";

    void Start(){
    }

    void Update(){

    }

    public void EquipItemToHand(string hand, ItemData itemData){
        var instance = Instantiate(itemData.prefab);
        if(!instance.TryGetComponent<Rigidbody>(out var rb)){
            Debug.LogError("Object does not have rigidbody, cancelling process.");
            Destroy(instance);
            return;
        }

        Transform handToUse;
        Rigidbody gameObjectToSave;

        if(hand == "left"){
            handToUse = leftHandBone;
            leftHandEquippedObject = rb;
            gameObjectToSave = leftHandEquippedObject;
        }else if(hand == "right"){
            handToUse = rightHandBone;
            rightHandEquippedObject = rb;
            gameObjectToSave = rightHandEquippedObject;
        }else{
            Debug.LogError("Error, no hand choosen correctly");
            return;
        }

        gameObjectToSave.gameObject.layer = LayerMask.NameToLayer(EquippedLayer);
        gameObjectToSave.transform.SetParent(handToUse, false);
        gameObjectToSave.transform.localPosition = Vector3.zero;
        gameObjectToSave.transform.localRotation = Quaternion.identity;
        gameObjectToSave.useGravity = false;
        gameObjectToSave.isKinematic = true;
        
    }

    public bool UnequipItemFromHand(string hand, InventoryItem item){
        if(hand == "left" && item.itemData.prefab.name + "(Clone)" == leftHandEquippedObject.name){
            Destroy(leftHandEquippedObject.gameObject);
        }else if(hand == "right" && item.itemData.prefab.name + "(Clone)" == rightHandEquippedObject.name){
            Destroy(rightHandEquippedObject.gameObject);
        }else{
            Debug.LogError("Error, no hand choosen correctly");
            return false;
        }

        return true;
    }

    public void DeleteHandsEquipment(){
        if(leftHandEquippedObject != null)
            Destroy(leftHandEquippedObject.gameObject);

        if(rightHandEquippedObject != null)
            Destroy(rightHandEquippedObject.gameObject);
    }

    void Reset(){
        _input = GetComponent<PlayerInputController>();
    }
}
