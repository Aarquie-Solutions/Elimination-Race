using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine;

namespace ZombieElimination
{
    public class ZombieAgent : MonoBehaviour
    {
        private FollowerEntity follower;
        private AIDestinationSetter destinationSetter;

        public Vector3 currentDestination;
        private Transform followTarget;

        [Header("Speed Control")]
        public float minSpeed = 2f;
        public float maxSpeed = 6f;
        public float preferredDistance = 10f;
        public float maxDistance = 17f;
        public float distanceTolerance = 2f;

        private AgentSpeedHandler speedHandler;


        void Awake()
        {
            follower = GetComponent<FollowerEntity>();
            if (!transform.TryGetComponent<AIDestinationSetter>(out destinationSetter))
            {
                destinationSetter = gameObject.AddComponent<AIDestinationSetter>();
            }
            speedHandler = GetComponent<AgentSpeedHandler>();
            if (speedHandler != null)
            {
                speedHandler.isPlayer = false;
            }
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            destinationSetter.target = followTarget;
        }

        private void Update()
        {
            if (followTarget == null) return;

            currentDestination = followTarget.position;

            float currentDist = Vector3.Distance(transform.position, followTarget.position);

            if (currentDist > maxDistance + distanceTolerance)
            {
                follower.maxSpeed = Mathf.Lerp(follower.maxSpeed, maxSpeed, Time.deltaTime * 2f);
            }
            else if (currentDist < preferredDistance - distanceTolerance)
            {
                follower.maxSpeed = Mathf.Lerp(follower.maxSpeed, minSpeed, Time.deltaTime * 2f);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentDestination, 0.5f);
            Gizmos.DrawLine(transform.position + Vector3.up, currentDestination + Vector3.up);
        }
    }
}
