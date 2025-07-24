using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace ZombieElimination
{
    public class Player : MonoBehaviour
    {
        public float Speed
        {
            get => follower.maxSpeed;
            private set => follower.maxSpeed = value;
        }

        public float targetSpeed;

        public List<Transform> waypoints;
        public float thresholdDistance = 6;

        private FollowerEntity follower;
        private int currentWaypointIndex = 0;

        private Vector3 currentDestination;
        public WaypointProgressTracker progressTracker;

        void Start()
        {
            waypoints = PathManager.Instance.GetRoutePoints();
            follower = GetComponent<FollowerEntity>();
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
