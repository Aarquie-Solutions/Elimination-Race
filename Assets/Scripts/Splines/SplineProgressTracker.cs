using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

public class SplineProgressTracker : MonoBehaviour
{
    public SplineContainer splineContainer;
    public Transform target;

    [Range(2, 64)] public int resolution = 16;
    [Range(1, 8)] public int iterations = 3;
    public float jumpThreshold = 0.15f; // Max allowed jump in normalized t
    public bool debug = true;

    private Spline spline;
    private float totalLength;
    private float previousT = 0f; // Normalized progress

    public float progress;
    public float normalizedT;

    void Start()
    {
        spline = splineContainer.Splines[0];
        totalLength = SplineUtility.CalculateLength(spline, splineContainer.transform.localToWorldMatrix);
    }

    void Update()
    {
        float3 localPos = splineContainer.transform.InverseTransformPoint(target.position);

        // Get nearest point on full spline
        SplineUtility.GetNearestPoint(
            spline,
            localPos,
            out float3 nearestPoint,
            out float rawT,
            resolution,
            iterations
        );

        // Convert to normalized t (in case spline has multiple segments)
        normalizedT = spline.ConvertIndexUnit(rawT, PathIndexUnit.Normalized);

        // Check for large jumps
        if (math.abs(normalizedT - previousT) > jumpThreshold)
        {
            // Keep previous or interpolate instead
            normalizedT = Mathf.Lerp(previousT, normalizedT, 0.25f); // Smoothing fallback
        }

        previousT = normalizedT;

        progress = normalizedT * totalLength;

        if (debug)
            Debug.Log($"Progress: {progress:F2}/{totalLength:F2} units | t={normalizedT:F3}");
    }
}
