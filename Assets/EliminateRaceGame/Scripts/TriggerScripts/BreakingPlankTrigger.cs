using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace EliminateRaceGame
{
    [RequireComponent(typeof(Collider))]
    public class BreakingPlankTrigger : ObstacleTrigger
    {
        public float breakDelay = 0.5f;
        private bool isTriggered;

        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        protected override void OnAgentTriggered(AgentController_RVO agent)
        {
            if (agent != null && !agent.isWinner && agent.currentLaneIndex == affectedLanes[0] && !isTriggered)
            {
                isTriggered = true;
                StartCoroutine(BreakPlank(agent));
            }
        }

        private IEnumerator BreakPlank(AgentController_RVO agent)
        {
            yield return new WaitForSeconds(breakDelay);
            Debug.Log($"{agent.gameObject.name} fell through breaking plank!");
            agent.Eliminate();
            gameObject.SetActive(false);
        }

        protected override void Initialise()
        {
            
        }

        protected override void OnTriggerStartActions(Collider other)
        {
        }

        protected override void OnTriggerEndActions(Collider other)
        {
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
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 0.2f, 1));
        }
    }
}
