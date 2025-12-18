using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivityUi : MonoBehaviour{
    [Header("References")]
    [SerializeField] private PlayerCameraController _plCameraCtrl;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text indicator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        if(_plCameraCtrl == null){
            Debug.LogError("Not found the player camera controller, UI sensitivity will be disabled");
            enabled = false;
            return;
        }
    }

    void Update(){
        indicator.text = slider.value.ToString();
    }

    void OnEnable(){
        slider.value = _plCameraCtrl.Sensibility;
    }

    public void ApplySensitivity(){
        _plCameraCtrl.Sensibility = slider.value;
    }
}
