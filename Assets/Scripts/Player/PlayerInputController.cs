using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour{
    [SerializeField] private PlayerInput playerInput;

    //Basic input
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInput;
    private bool attackInput;
    private bool interactInput;
    private bool useObjectInput;

    //Input Actions
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction lookAction;
    [HideInInspector] public InputAction sprintAction;
    [HideInInspector] public InputAction attackAction;
    [HideInInspector] public InputAction interactAction;
    [HideInInspector] public InputAction useObjectAction;


    //Public References
    public Vector2 MoveInput {get {return moveInput;}}
    public Vector2 LookInput {get {return lookInput;}}
    public bool SprintInput {get {return sprintInput;}}
    public bool AttackInput {get {return attackInput;}}
    public bool InteractInput {get {return interactInput;}}
    public bool UseObjectInput {get {return useObjectInput;}}

    public string CurrentControlScheme {get{return playerInput.currentControlScheme;}}

    void Start(){
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];
        attackAction = playerInput.actions["Attack"];
        interactAction = playerInput.actions["Interact"];
        useObjectAction = playerInput.actions["UseObject"];

        sprintAction.performed += (ctx) => sprintInput = true;
        sprintAction.canceled += (ctx) => sprintInput = false;

        attackAction.performed += (ctx) => attackInput = true;
        attackAction.canceled += (ctx) => attackInput = false;

        interactAction.performed += (ctx) => interactInput = true;
        interactAction.canceled += (ctx) => interactInput = false;

        useObjectAction.performed += (ctx) => useObjectInput = true;
        useObjectAction.canceled += (ctx) => useObjectInput = false;
    }

    public void OnMove(InputValue ctx){
        moveInput = ctx.Get<Vector2>();
    }

    public void OnLook(InputValue ctx){
        lookInput = ctx.Get<Vector2>();
    }

    void Reset(){
        playerInput = GetComponent<PlayerInput>();
    }
}
