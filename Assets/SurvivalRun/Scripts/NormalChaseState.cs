using UnityEngine;

namespace ZombieElimination
{
    public class NormalChaseState : IZombieState
    {
        public void EnterState(ZombieAgent agent)
        {
            // Optional: Trigger chasing animation
            if (agent.Animator != null)
            {
                agent.Animator.SetBool("IsEliminating", false);
            }
            agent.Follower.maxSpeed = agent.SpeedHandler.maxSpeed;
            agent.Follower.rvoSettings.priority = 0.2f;
        }

        public void UpdateState(ZombieAgent agent)
        {
            if (agent.FollowTarget == null) return;

            float distance = Vector3.Distance(agent.transform.position, agent.FollowTarget.position);

            if (distance > agent.maxDistance + agent.distanceTolerance)
            {
                agent.SpeedHandler.SetTargetSpeed(agent.SpeedHandler.maxSpeed);
            }
            else if (distance < agent.preferredDistance - agent.distanceTolerance)
            {
                agent.SpeedHandler.SetTargetSpeed(agent.SpeedHandler.minSpeed);
            }
        }

        public void ExitState(ZombieAgent agent)
        {
        }
    }
}
