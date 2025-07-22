using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

[ExecuteInEditMode, DisallowMultipleComponent]
public class LaneNodesEditor : MonoBehaviour
{
    public bool validate;

    private SplineContainer splineContainer;
    public List<Transform> nodes;

    public float distanceBetweenNodes = 2f;
    public Transform nodePrefab;

    public void OnValidate()
    {
        if (validate)
        {
            validate = false;
            SetUpNodes();
        }
    }
#if UNITY_EDITOR


    private void SetUpNodes()
    {
        splineContainer ??= GetComponent<SplineContainer>();
        if (nodePrefab == null)
        {
            if (transform.childCount > 0)
            {
                nodePrefab = transform.GetChild(0);
            }
            else
            {
                nodePrefab = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                nodePrefab.parent = transform;
                CheckAndDeleteCollider(nodePrefab);
            }
        }
        if (nodes == null || nodes.Count != transform.childCount)
        {
            nodes = GetComponentsInChildren<Transform>().ToList();
            if (nodes[0] == transform)
            {
                nodes.RemoveAt(0);
            }
        }

        float totalLength = splineContainer.Spline.CalculateLength(transform.localToWorldMatrix);
        int nodeCount = Mathf.CeilToInt(totalLength / distanceBetweenNodes);
        float distanceAt = 0;
        for (int i = 0; i < nodeCount; i++)
        {
            Transform tr;
            if (i > nodes.Count - 1)
            {
                tr = Instantiate(nodePrefab, transform);
                nodes.Add(tr);
                Undo.RegisterCreatedObjectUndo(tr.gameObject, "Create Node");
            }
            else
            {
                tr = nodes[i];
                Undo.RecordObject(tr, "Update Node");
            }
            CheckAndTurnOffShadows(tr);
            CheckAndDeleteCollider(tr);
            var time = Mathf.Clamp01(distanceAt / totalLength);
            tr.position = splineContainer.Spline.EvaluatePosition(time);
            tr.forward = splineContainer.Spline.EvaluateTangent(time);
            if (i < nodeCount - 1)
            {
                distanceAt += distanceBetweenNodes;
            }
        }

        if (nodes.Count > nodeCount)
        {
            for (int j = nodes.Count - 1; j >= nodeCount; j--)
            {
                if (nodes[j] != null)
                {
                    Transform toDelete = nodes[j];
                    Undo.RecordObject(toDelete, "Delete Node");
                    nodes.RemoveAt(j);
                    EditorApplication.delayCall += () => { DestroyImmediate(toDelete.gameObject); };
                }
            }
        }
        if (distanceAt < totalLength)
        {
            var tr = Instantiate(nodePrefab, transform);
            nodes.Add(tr);
            CheckAndDeleteCollider(tr);
            Undo.RegisterCreatedObjectUndo(tr.gameObject, "Create Node");
            tr.position = splineContainer.Spline.EvaluatePosition(1);
            tr.forward = splineContainer.Spline.EvaluateTangent(1);
        }
    }

    public void CheckAndDeleteCollider(Transform tr)
    {
        var col = tr.GetComponent<Collider>();
        if (col != null)
        {
            EditorApplication.delayCall += () => { DestroyImmediate(col); };
        }
    }

    public void CheckAndTurnOffShadows(Transform tr)
    {
        var rend = tr.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.shadowCastingMode = ShadowCastingMode.Off;
        }
    }
#endif
}
