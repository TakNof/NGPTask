using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovementController : MonoBehaviour{
    [Header("References")]
    [SerializeField] private CharacterController _charCtrl;
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private Camera _mainCamera;

    [Header("Data")]
    [Header("Movement")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float targetSpeed;

    [Header("Rotation")]
    [SerializeField] private float targetMovement;
    [SerializeField] private float targetRotation;
    [SerializeField] private float movementRotationVelocity;

    [Header("Settings")]
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [Header("Rotation")]
    [SerializeField] private float movementRotationSmoothTime;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded;
    [HideInInspector] public bool IsGrounded {get{return isGrounded;}}
    [SerializeField] private float groundedOffset;
    [SerializeField] private float  groundedRadius;
    [SerializeField] private LayerMask groundLayers;

    //gizmos test colors
    private readonly Color transparentGreen =new(0.0f, 1.0f, 0.0f, 0.35f);
    private readonly Color transparentRed =new(1.0f, 0.0f, 0.0f, 0.35f);

    // Update is called once per frame
    void Update(){
        HandleMovement();
    }

    private void FixedUpdate(){
        GroundedCheck();
        HandleGravity();
    }

    private void HandleGravity(){
        _charCtrl.Move(Physics.gravity * Time.deltaTime);
    }

    private void HandleMovement(){
        if(_input.MoveInput == Vector2.zero || !isGrounded) return;

        targetSpeed = _input.SprintInput ? runSpeed : walkSpeed;

        currentSpeed = targetSpeed;

        var NormalizedMoveInput = new Vector3(_input.MoveInput.x, 0, _input.MoveInput.y).normalized;

        var currentMovementDirection = Mathf.Atan2(NormalizedMoveInput.x, NormalizedMoveInput.z) * Mathf.Rad2Deg;

        targetMovement = currentMovementDirection + _mainCamera.transform.eulerAngles.y;
        targetRotation = targetMovement; //For now, I'll see if I can implement the other things first

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref movementRotationVelocity,
                    movementRotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

        var targetDirection = Quaternion.Euler(0.0f, targetMovement, 0.0f) * Vector3.forward; 
        _charCtrl.Move(currentSpeed * Time.deltaTime * targetDirection.normalized);
    }

    private void GroundedCheck(){
        // set sphere position, with offset
        Vector3 spherePosition = new(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmosSelected(){
        Gizmos.color = isGrounded ? transparentGreen : transparentRed;

        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z),
            groundedRadius);
    }

    void Reset(){
        _charCtrl = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputController>();
        _mainCamera = Camera.main;
    }
}
