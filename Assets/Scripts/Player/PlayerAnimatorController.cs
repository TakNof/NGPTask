using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
[RequireComponent(typeof(PlayerInteractionController))]
[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour{
    [SerializeField] private PlayerMovementController _movementCtrl;
    [SerializeField] private PlayerInteractionController _interactionCtrl;
    [SerializeField] private Animator _animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        _movementCtrl.OnGroundedStateChanged.AddListener(UpdateGroundedBool);
    }

    // Update is called once per frame
    void Update(){
        UpdateSpeedFloat();
    }
    
    private void UpdateGroundedBool(bool value){
        _animator.SetBool("isGrounded", value);
    }

    private void UpdateSpeedFloat(){
        _animator.SetFloat("Speed", _movementCtrl.CurrentSpeed);
    }

    void Reset(){
        _movementCtrl = GetComponent<PlayerMovementController>();
        _interactionCtrl = GetComponent<PlayerInteractionController>();
        _animator = GetComponent<Animator>();
    }
}
