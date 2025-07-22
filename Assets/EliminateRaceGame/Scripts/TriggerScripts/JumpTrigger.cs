using System;

namespace EliminateRaceGame
{
    using UnityEngine;

    public class JumpTrigger : ObstacleTrigger
    {
        public float jumpHeight = 2f;
        public float jumpDuration = 0.8f;

        public Transform jumpStartPoint, jumpEndPoint;

        public int selectToDie = 3;
        private Collider col;

        private void OnValidate()
        {
            col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        protected override void Initialise()
        {
            EliminationTag = EliminationTag.Jump;
            MaxAgentsToEliminate = Random.Range(1, selectToDie + 1);
        }

        protected override void OnAgentTriggered(AgentController_RVO agent)
        {
            if (!agent.isWinner)
            {
                if (selectToDie-- > 0)
                {
                    agent.eliminationTag = EliminationTag.Jump;
                }
            }
            agent.StartJump(jumpStartPoint.position, jumpEndPoint.position, jumpHeight, jumpDuration);
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
            if (jumpStartPoint != null && jumpEndPoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
                Gizmos.DrawSphere(jumpStartPoint.position, 0.2f);
                Gizmos.DrawSphere(jumpEndPoint.position, 0.2f);
                Gizmos.DrawLine(jumpStartPoint.position, jumpEndPoint.position);
            }
        }
    }
}
