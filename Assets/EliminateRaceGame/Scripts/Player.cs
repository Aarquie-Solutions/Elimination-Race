using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace ZombieElimination
{
    public class Player : MonoBehaviour
    {

        public List<Transform> waypoints;
        public float thresholdDistance = 6;

        private FollowerEntity follower;
        private int currentWaypointIndex = 0;

        private Vector3 currentDestination;
        public WaypointProgressTracker progressTracker;


        private AgentSpeedHandler speedHandler;

        void Awake()
        {
            follower = GetComponent<FollowerEntity>();
            speedHandler = GetComponent<AgentSpeedHandler>();
            if (speedHandler != null)
            {
                speedHandler.isPlayer = true;
            }
        }

        void Start()
        {
            waypoints = PathManager.Instance.GetRoutePoints();
            progressTracker = new WaypointProgressTracker(waypoints, transform);
            if (waypoints.Count > 0)
            {
                SetDestination(waypoints[0].position);
            }
        }

        void Update()
        {
            if (follower == null || waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count) return;
            progressTracker.UpdateProgress();
            float dist = Vector3.Distance(transform.position, currentDestination);
            if (dist < thresholdDistance)
            {
                AdvanceToNextWaypoint();
            }
        }

        private void AdvanceToNextWaypoint()
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex -= 1;
            }
            SetDestination(PathManager.GetRandomNavmeshPointNear(waypoints[currentWaypointIndex]));
        }

        private void SetDestination(Vector3 position)
        {
            currentDestination = position;
            follower.destination = position;
        }
    }
}
