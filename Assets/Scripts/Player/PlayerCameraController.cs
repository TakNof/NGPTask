using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(PlayerInputController))]
public class PlayerCameraController : MonoBehaviour{
    [Header("References")]
    [SerializeField] private PlayerInputController _input;
    [SerializeField] private CinemachineCamera _cmThirdPersonCamera;
    [SerializeField] private Transform _cmCameraGuide;

    [Header("Settings")]
    public float Sensibility = 5f;
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

    void Start(){
        if (_cmThirdPersonCamera?.Target != null)
            _cmThirdPersonCamera.Target.TrackingTarget ??= _cmCameraGuide;
    }

    void Update(){
        if(!Cursor.visible)
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

    void Reset(){
        _input = GetComponent<PlayerInputController>();
    }
}
