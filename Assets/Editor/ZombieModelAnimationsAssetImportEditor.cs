using UnityEngine;
using UnityEditor;

public class ZombieModelAnimationsAssetImportEditor : EditorWindow
{
    private UnityEngine.Object selectedFolderOrModel;
    private bool setRootRotation = true;
    private bool setRootPositionY = true;
    private bool setRootPositionXZ = true;

    [MenuItem("Tools/Animation Import Utility")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ZombieModelAnimationsAssetImportEditor), false, "Animation Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Model or Folder Containing FBX Files", EditorStyles.boldLabel);

        selectedFolderOrModel = EditorGUILayout.ObjectField("Target Folder or File", selectedFolderOrModel, typeof(UnityEngine.Object), false);

        GUILayout.Space(10);
        GUILayout.Label("Root Transform Settings", EditorStyles.boldLabel);

        setRootRotation = EditorGUILayout.Toggle("Bake Root Rotation", setRootRotation);
        setRootPositionY = EditorGUILayout.Toggle("Bake Root Position Y", setRootPositionY);
        setRootPositionXZ = EditorGUILayout.Toggle("Bake Root Position XZ", setRootPositionXZ);

        GUILayout.Space(20);

        if (GUILayout.Button("Apply Settings"))
        {
            ApplySettings();
        }
    }

    private void ApplySettings()
    {
        if (selectedFolderOrModel == null)
        {
            Debug.LogWarning("No target selected.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selectedFolderOrModel);

        string[] fbxFiles;

        if (AssetDatabase.IsValidFolder(path))
        {
            fbxFiles = AssetDatabase.FindAssets("t:Model", new[] { path });
        }
        else if (path.EndsWith(".fbx"))
        {
            fbxFiles = new[] { AssetDatabase.AssetPathToGUID(path) };
        }
        else
        {
            Debug.LogWarning("Please select a folder or an FBX file.");
            return;
        }

        foreach (var guid in fbxFiles)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (modelImporter == null)
            {
                Debug.LogWarning($"Invalid model importer for {assetPath}");
                continue;
            }

            var clips = modelImporter.clipAnimations;
            if (clips == null || clips.Length == 0)
            {
                clips = modelImporter.defaultClipAnimations;
            }

            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].keepOriginalOrientation = !setRootRotation;
                clips[i].keepOriginalPositionY = !setRootPositionY;
                clips[i].keepOriginalPositionXZ = !setRootPositionXZ;
            }

            modelImporter.clipAnimations = clips;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            Debug.Log($"Applied root settings to: {assetPath}");
        }

        Debug.Log("Finished applying animation import settings.");
    }
}
