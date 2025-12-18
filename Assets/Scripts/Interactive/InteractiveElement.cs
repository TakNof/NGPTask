using UnityEngine;
[RequireComponent(typeof(Collider))]
public class InteractiveElement : MonoBehaviour{
    public ItemData itemData;

    private readonly string layerName = "Interactive";
    private readonly string highlightLayerName = "InteractiveHighlight";

    public string GetName(){
        return itemData.name;
    }

    public void SetHighlight(){
        SetLayer(highlightLayerName);
    }

    public void SetNormal(){
        SetLayer(layerName);
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