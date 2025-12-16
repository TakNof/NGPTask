using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;

[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(PlayerMovementController))]
public class PlayerCameraController : MonoBehaviour{
    [Header("References")]
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private PlayerMovementController _movementCtrl;
    [SerializeField] private CinemachineCamera _cmThirdPersonCamera;
    [SerializeField] private Transform _cmCameraGuide;

    [Header("Settings")]
    [SerializeField] private float Sensibility = 1f;
    [SerializeField] private float TopClamp = 70.0f;
    [SerializeField] private float BottomClamp = -30.0f;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private bool IsCurrentDeviceMouse{
        get{
            #if ENABLE_INPUT_SYSTEM
                return _input.CurrentControlScheme == "KeyboardMouse";
            #else
                return false;
            #endif
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        if (_cmThirdPersonCamera?.Target != null)
            _cmThirdPersonCamera.Target.TrackingTarget ??= _cmCameraGuide;

        AllowCursor(false);
    }

    // Update is called once per frame
    void Update(){
        HandleCamera();
    }

    private void HandleCamera(){
        if(_input.MoveInput == Vector2.zero && _input.LookInput == Vector2.zero) return;
        float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

        _cinemachineTargetYaw += deltaTimeMultiplier * Sensibility * _input.LookInput.x;
        _cinemachineTargetPitch -= deltaTimeMultiplier * Sensibility * _input.LookInput.y;

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        _cmCameraGuide.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax){
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void AllowCursor(bool state){
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    void Reset(){
        _input = GetComponent<PlayerInputController>();
        _movementCtrl = GetComponent<PlayerMovementController>();
    }
}
