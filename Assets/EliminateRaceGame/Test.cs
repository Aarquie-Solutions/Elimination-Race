using System;
using UnityEngine;
using UnityEngine.Splines;

public class Test : MonoBehaviour
{
    public SplineContainer splineContainer;

    public float t;

    public float totalLength2;
    public Transform target;

    public float curr, curr2;

    public bool start;
    private void OnValidate()
    {
        totalLength2 = SplineUtility.CalculateLength(splineContainer.Spline, splineContainer.transform.localToWorldMatrix);
    }

    private void OnDrawGizmos()
    {
        SplineUtility.Evaluate(splineContainer.Spline, Mathf.InverseLerp(0, totalLength2, t), out var pos, out var tangent, out var normal);
        // Vector3 s = SplineUtility.EvaluatePosition(splineContainer.Spline, Mathf.InverseLerp(0, totalLength2, t));
        // s = splineContainer.transform.TransformPoint(s);
        target.position = splineContainer.transform.TransformPoint(pos);
        target.forward = tangent;
    }

    public float axisLength = 1f;

    public void FixedUpdate()
    {
        if (!start) return;
        curr = Mathf.Lerp(curr, 500, Time.deltaTime);
        curr2 = Mathf.MoveTowards(curr, 500, Time.deltaTime);
    }
}
