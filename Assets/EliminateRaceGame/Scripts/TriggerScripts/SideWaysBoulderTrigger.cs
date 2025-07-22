using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace EliminateRaceGame
{
    [RequireComponent(typeof(Collider))]
    public class SidewaysBoulderTrigger : ObstacleTrigger
    {
        public Vector3 direction = Vector3.right;
        public float speed = 3f;
        public float distance = 10f;
        public Vector3 startPos;
        private float t;

        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }


        protected override void Initialise()
        {
            startPos = transform.position;
        }

        protected override void OnTriggerStartActions(Collider other)
        {
            
        }

        protected override void OnTriggerEndActions(Collider other)
        {
            
        }

        private void Update()
        {
            t = (t + Time.deltaTime * speed / distance) % 1f;
            transform.position = startPos + direction * Mathf.Sin(t * Mathf.PI) * distance;
        }

        protected override void OnAgentTriggered(AgentController_RVO agent)
        {
            if (agent != null && !agent.isWinner && affectedLanes.Contains(agent.currentLaneIndex))
            {
                Debug.Log($"{agent.gameObject.name} hit by sideways boulder!");
                agent.Eliminate();
            }
        }

        protected override bool CanActivateCondition()
        {
            return false;
        }

        protected override void ActivateObstacle()
        {
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 1f);
            Gizmos.DrawLine(startPos, startPos + direction * distance);
        }
    }
}
