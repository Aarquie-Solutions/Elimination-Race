using UnityEngine;
using UnityEditor;

public class OnTriggerEventEditor
{
    [MenuItem("GameObject/Create Trigger Child", false, 0)]
    private static void CreateTriggerChild(MenuCommand command)
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            Debug.LogWarning("No GameObject selected in the Hierarchy.");
            return;
        }

        // Create the new GameObject
        GameObject triggerGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        triggerGO.name = "TriggerZone";
        triggerGO.transform.SetParent(selected.transform);
        triggerGO.transform.localPosition = Vector3.zero;

        // Add components
        if (!triggerGO.TryGetComponent(out BoxCollider collider))
            collider = triggerGO.AddComponent<BoxCollider>();

        if (!triggerGO.TryGetComponent(out Rigidbody rb))
            rb = triggerGO.AddComponent<Rigidbody>();

        if (!triggerGO.TryGetComponent(out OnTriggerEvent ote))
            ote = triggerGO.AddComponent<OnTriggerEvent>();

        if (triggerGO.TryGetComponent(out MeshRenderer mr))
            mr.enabled = false;

        rb.useGravity = false;
        rb.isKinematic = false;
        collider.isTrigger = true;

        Selection.activeGameObject = triggerGO;
    }

    // Only show menu when right-clicking a GameObject in Hierarchy
    [MenuItem("GameObject/Create Trigger Child", true)]
    private static bool ValidateCreateTriggerChild()
    {
        return Selection.activeGameObject != null;
    }
}
