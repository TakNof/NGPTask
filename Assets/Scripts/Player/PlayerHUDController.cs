using UnityEngine;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerHUDController : MonoBehaviour{
    [Header("References")]
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private Canvas HUDCanvas;
    [SerializeField] private Canvas InventoryCanvas;

    private Canvas CurrentDisplayedCanvas;

    void Start(){
        _input.openInventoryAction.performed += (ctx) => ShowInventory();
        _input.cancelAction.performed += (ctx) => HideDisplayedCanvas();

        AllowCursor(false);
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

    public void AllowCursor(bool state){
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void Reset(){
        _input = GetComponent<PlayerInputController>();
    }
}
