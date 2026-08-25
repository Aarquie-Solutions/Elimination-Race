using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaypointProgressTracker
{
    public int currentSegmentIndex = 0;
    public float totalProgress = 0f;

    private static float updateInterval = 0.3f;
    private float updateTimer;

    private List<Transform> waypoints;
    private Transform transform;

    public WaypointProgressTracker(List<Transform> waypoints, Transform rootTransform)
    {
        this.waypoints = waypoints;
        this.transform = rootTransform;
    }

    public void UpdateProgress()
    {
        updateTimer += Time.deltaTime;
        
        if (updateTimer < updateInterval) return;
        updateTimer = 0f;
        
        if (waypoints == null || waypoints.Count < 2) return;

        int nextIndex = Mathf.Clamp(currentSegmentIndex + 1, 0, waypoints.Count - 1);

        Vector3 a = waypoints[currentSegmentIndex].position;
        Vector3 b = waypoints[nextIndex].position;
        Vector3 pos = transform.position;

        Vector3 ab = b - a;
        Vector3 ap = pos - a;

        float segmentLength = ab.magnitude;
        float projection = Vector3.Dot(ap, ab.normalized);

        if (projection >= segmentLength && currentSegmentIndex < waypoints.Count - 2)
        {
            currentSegmentIndex++;
            UpdateProgress();
            return;
        }

        projection = Mathf.Clamp(projection, 0f, segmentLength);

        float distance = 0f;
        for (int i = 0; i < currentSegmentIndex; i++)
        {
            distance += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        }

        distance += projection;
        totalProgress = distance;
    }
}
