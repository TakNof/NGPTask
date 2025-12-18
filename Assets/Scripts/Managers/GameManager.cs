using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable(){
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable(){
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode){
        LoadInventory();
    }

    public void ExitGame(){
        SaveInventory();
        Application.Quit();
    }

    public void ShowElement(GameObject element){
        element.SetActive(true);
    }

    public void HideElement(GameObject element){
        element.SetActive(false);
    }

    public void SaveInventory(){
        var data = InventoryManager.Instance.GetSaveData();
        string json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(
            Application.persistentDataPath + "/inventory.json",
            json
        );

        Debug.Log("Inventory has been saved");
    }

    public void LoadInventory(){
        string path = Application.persistentDataPath + "/inventory.json";
        if (!System.IO.File.Exists(path)) return;

        string json = System.IO.File.ReadAllText(path);
        var data = JsonUtility.FromJson<InventoryManager.InventorySaveData>(json);

        InventoryManager.Instance.LoadInventory(data);

        Debug.Log("Inventory has been loaded");
    }

    void OnDestroy(){
        if (Instance == this)
            Instance = null;
    }
}
