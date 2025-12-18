using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(PlayerInteractionController))]
public class PlayerHUDController : MonoBehaviour{
    [Header("References")]
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private PlayerInteractionController _interactionCtrl;
    [SerializeField] private Canvas HUDCanvas;
    [SerializeField] private Canvas InventoryCanvas;

    [SerializeField] private TMP_Text indicativeText;

    private Canvas CurrentDisplayedCanvas;
    private readonly string HintText = "Hold F to grab ";

    void Start(){
        _input.openInventoryAction.performed += (ctx) => ShowInventory();
        _input.cancelAction.performed += (ctx) => HideDisplayedCanvas();

        // _interactionCtrl.OnInteractiveEntered.AddListener(ShowHint);
        // _interactionCtrl.OnInteractiveExited.AddListener(HideHint);

        _interactionCtrl.OnDetectedObjectsUpdated.AddListener((ie) =>{
            if(ie == null){
                HideHint(ie);
            }else{
                ShowHint(ie);
            }
        });

        AllowCursor(false);
        ToggleHintGameObject(false);
    }

    // Update is called once per frame
    void Update(){
        
    }

    private void SetUIState(Canvas enable, Canvas disable, bool allowCursor){
        if(enable == null || disable == null){
            Debug.LogError("There are invalid canvases into the switch UI state method");
            return;
        }

        if (enable == null){
            Debug.LogError("The canvas to enable list is invalid");
            return;
        }
        enable.gameObject.SetActive(true);
            
            

        if (disable == null){
            Debug.LogError("The canvas to disable list is invalid");
            return;
        }
        disable.gameObject.SetActive(false);
            
        AllowCursor(allowCursor);

        if (allowCursor){
            _input.ChangeToUIMap();
            CurrentDisplayedCanvas = enable;
        }else{
            _input.ChangeToPlayerMap();
            CurrentDisplayedCanvas = null;
        }
    }

    private void ShowInventory(){
        SetUIState(InventoryCanvas, HUDCanvas, true);
    }

    private void HideDisplayedCanvas(){
        SetUIState(HUDCanvas, CurrentDisplayedCanvas, false);
    }

    private void ShowHint(InteractiveElement interactiveElement){
        ShowHintText(interactiveElement);
        ToggleHintGameObject(true);
    }

    private void HideHint(InteractiveElement interactiveElement){
        HideHintText(interactiveElement);
        ToggleHintGameObject(false);
    }

    private void ShowHintText(InteractiveElement interactiveElement){
        indicativeText.text = "Hold F to grab " + interactiveElement.itemData.name;
    }

    private void HideHintText(InteractiveElement interactiveElement){
        indicativeText.text = null;
    }

    private void ToggleHintGameObject(bool state){
        indicativeText.transform.parent.gameObject.SetActive(state);
    }

    public void AllowCursor(bool state){
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void Reset(){
        _input = GetComponent<PlayerInputController>();
        _interactionCtrl = GetComponent<PlayerInteractionController>();
    }
}
