#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
public class AnimationExtractor : EditorWindow
{
    private string folderPath = "Assets/"; // Carpeta de los FBX
    private string outputFolder = "Assets/ExtractedAnimations/"; // Carpeta donde se guardarán las animaciones

    [MenuItem("Tools/Extract Animations from FBX")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AnimationExtractor), false, "Animation Extractor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Extract Animations from FBX", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("FBX Folder Path:", folderPath);
        outputFolder = EditorGUILayout.TextField("Output Folder Path:", outputFolder);

        if (GUILayout.Button("Extract Animations"))
        {
            ExtractAnimations();
        }
    }

    private void ExtractAnimations()
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Folder not found: " + folderPath);
            return;
        }

        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        string[] fbxFiles = Directory.GetFiles(folderPath, "*.fbx");

        foreach (string fbxFile in fbxFiles)
        {
            string assetPath = fbxFile.Replace(Application.dataPath, "Assets");

            ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (modelImporter == null)
            {
                Debug.LogError("Could not get ModelImporter for: " + assetPath);
                continue;
            }

            // Forzar la importación para asegurarnos de que tenemos las animaciones
            modelImporter.importAnimation = true;
            modelImporter.SaveAndReimport();

            // Cargar las animaciones desde el FBX
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning($"No assets found in: {assetPath}");
                continue;
            }

            bool animationFound = false;
            string fbxName = Path.GetFileNameWithoutExtension(fbxFile);

            foreach (Object asset in assets)
            {
                if (asset is AnimationClip clip && !clip.name.Contains("__preview__"))
                {
                    animationFound = true;
                    string clipPath = Path.Combine(outputFolder, fbxName + ".anim");
                    clipPath = AssetDatabase.GenerateUniqueAssetPath(clipPath);

                    AnimationClip newClip = new AnimationClip();
                    EditorUtility.CopySerialized(clip, newClip);
                    AssetDatabase.CreateAsset(newClip, clipPath);

                    Debug.Log($"Extracted: {clipPath}");
                }
            }

            if (!animationFound)
            {
                Debug.LogWarning($"No animations found in: {assetPath}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
