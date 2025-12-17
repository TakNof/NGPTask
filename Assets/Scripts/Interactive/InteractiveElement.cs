using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class InteractiveElement : MonoBehaviour{
    private readonly string layerName = "Interactive";
    private readonly string highlightLayerName = "InteractiveHighlight";

    void Start(){
        
    }

    void Update(){
        
    }

    private void SetLayer(string layerName){
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    void Reset(){
        SetLayer(layerName);
    }

    [ContextMenu("Delete all interaction components")]
    private void DeleteAllInteractionComponents() {
        Debug.Log("All interaction components were deleted.");
        SetLayer("Default");
        DestroyImmediate(this);
    }
}