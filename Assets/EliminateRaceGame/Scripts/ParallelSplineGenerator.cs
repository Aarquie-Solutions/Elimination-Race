// ParallelSplineGenerator.cs

using UnityEngine;
using UnityEngine.Splines;
using UnityEditor;
using System.Collections.Generic;
using EliminateRaceGame;
using Unity.VisualScripting;

namespace EliminateRaceGame
{
    public class ParallelSplineGenerator : MonoBehaviour
    {
        public SplineContainer centerSpline;
        public float laneOffset = 2.0f; // Half-width of agent * 2
        public int laneCount = 2; // Total lanes: even number like 2 or 4
        public int sampleCount = 50;
        public LaneManager laneManager;

        [ContextMenu("Generate Parallel Lanes")]
        public void GenerateLanes()
        {
            if (centerSpline == null || laneCount < 1)
            {
                Debug.LogError("Invalid input.");
                return;
            }
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                if (Application.isEditor)
                    DestroyImmediate(transform.GetChild(i).gameObject);
                else
                    Destroy(transform.GetChild(i).gameObject);
            }


            var center = centerSpline.Spline;
            float length = SplineUtility.CalculateLength(center, centerSpline.transform.localToWorldMatrix);
            laneManager.ClearLanes();
            for (int laneIndex = 0; laneIndex < laneCount; laneIndex++)
            {
                // -1, 0, 1... offset from center
                float laneSideOffset = ((laneIndex + 0.5f) - laneCount / 2f) * laneOffset;

                List<BezierKnot> newKnots = new List<BezierKnot>();

                for (int i = 0; i <= sampleCount; i++)
                {
                    float t = i / (float)sampleCount;
                    float dist = t * length;

                    if (SplineUtility.Evaluate(center, Mathf.InverseLerp(0, length, dist), out var pos, out var tangent, out var up))
                    {
                        Vector3 normal = Vector3.Cross(up, tangent).normalized;
                        Vector3 offsetPos = (Vector3)pos + normal * laneSideOffset;
                        // Simplified bezier knot
                        var knot = new BezierKnot(offsetPos);
                        knot.Rotation = Quaternion.LookRotation(tangent, Vector3.up);
                        newKnots.Add(knot);
                    }
                }

                // Create a new GameObject with SplineContainer for this lane
                GameObject laneGO = new GameObject($"LaneSpline_{laneIndex}");
                laneGO.transform.SetParent(this.transform);
                var laneContainer = laneGO.AddComponent<SplineContainer>();
                laneContainer.Spline.Clear();
                for (int i = 0; i < newKnots.Count; i++)
                {
                    laneContainer.Spline.Add(newKnots[i]);
                }
                var lne = laneContainer.AddComponent<LaneNodesEditor>();
                lne.validate = true;
                lne.OnValidate();
                laneManager.AddLane(laneContainer);
            }

            Debug.Log("Lanes generated.");
        }
    }
}
