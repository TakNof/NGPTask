using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour{
    [SerializeField] private PlayerInput playerInput;

    //Basic input
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintInput;
    // private bool attackInput;
    // private bool interactInput;
    // private bool useObjectInput;
    // private bool pauseInput;
    // private bool openInventoryInput;

    //Input Actions
    //Player
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction lookAction;
    [HideInInspector] public InputAction sprintAction;
    [HideInInspector] public InputAction attackAction;
    [HideInInspector] public InputAction interactAction;
    [HideInInspector] public InputAction useObjectAction;
    [HideInInspector] public InputAction pauseAction;
    [HideInInspector] public InputAction openInventoryAction;

    //UI
    [HideInInspector] public InputAction cancelAction;
    [HideInInspector] public InputAction dropItemAction;


    //Public References
    public Vector2 MoveInput {get {return moveInput;}}
    public Vector2 LookInput {get {return lookInput;}}
    public bool SprintInput {get {return sprintInput;}}
    // public bool AttackInput {get {return attackInput;}}
    // public bool InteractInput {get {return interactInput;}}
    // public bool UseObjectInput {get {return useObjectInput;}}
    // public bool PauseInput {get{return pauseInput;}}
    // public bool OpenInventoryInput {get{return openInventoryInput;}}

    //ActionMaps
    private InputActionMap playerMap;
    private InputActionMap uiMap;


    public string CurrentControlScheme {get{return playerInput.currentControlScheme;}}

    void Awake(){
        playerMap = playerInput.actions.FindActionMap("Player");
        uiMap = playerInput.actions.FindActionMap("UI");

        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];
        attackAction = playerInput.actions["Attack"];
        interactAction = playerInput.actions["Interact"];
        useObjectAction = playerInput.actions["UseObject"];
        pauseAction = playerInput.actions["Pause"];
        openInventoryAction = playerInput.actions["OpenInventory"];

        sprintAction.performed += (ctx) => sprintInput = true;
        sprintAction.canceled += (ctx) => sprintInput = false;

        cancelAction = playerInput.actions["Cancel"];
        dropItemAction = playerInput.actions["DropItem"]; 

        // attackAction.performed += (ctx) => attackInput = true;
        // attackAction.canceled += (ctx) => attackInput = false;

        // interactAction.performed += (ctx) => interactInput = true;
        // interactAction.canceled += (ctx) => interactInput = false;

        // useObjectAction.performed += (ctx) => useObjectInput = true;
        // useObjectAction.canceled += (ctx) => useObjectInput = false;

        // pauseAction.performed += (ctx) => pauseInput = !pauseInput;

        // openInventoryAction.performed += (ctx) => openInventoryInput = !openInventoryInput;

        playerMap.Enable();
        uiMap.Disable();
    }

    public void OnMove(InputValue ctx){
        moveInput = ctx.Get<Vector2>();
    }

    public void OnLook(InputValue ctx){
        lookInput = ctx.Get<Vector2>();
    }

    public void ChangeToUIMap(){
        playerMap.Disable();
        uiMap.Enable();
    }

    public void ChangeToPlayerMap(){
        uiMap.Disable();
        playerMap.Enable();
    }

    void Reset(){
        playerInput = GetComponent<PlayerInput>();
    }
}
