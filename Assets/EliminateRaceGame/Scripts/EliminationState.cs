using System.Collections;
using Pathfinding.RVO;

namespace ZombieElimination
{
    using UnityEngine;

    public class EliminationState : IZombieState
    {
        private readonly float eliminationSpeed;
        public Player playerToEliminate;
        private ZombieAgent agent;

        public EliminationState(float eliminationSpeed)
        {
            this.eliminationSpeed = eliminationSpeed;
        }

        public void EnterState(ZombieAgent agent)
        {
            this.agent = agent;
            // playerToEliminate.StartElimination();

            if (biteCoroutine != null)
            {
                agent.StopCoroutine(biteCoroutine);
            }
            biteCoroutine = null;
            agent.Follower.maxSpeed = eliminationSpeed;

            agent.Follower.rvoSettings.priority = 1;

            if (agent.Animator != null)
            {
                //  agent.Animator.SetBool("IsEliminating", true);
            }
        }

        private Coroutine biteCoroutine;

        public void UpdateState(ZombieAgent agent)
        {
            if (agent.FollowTarget == null) return;

            agent.SpeedHandler.SetTargetSpeed(eliminationSpeed);

            if (biteCoroutine == null)
            {
                agent.FollowTarget.position = playerToEliminate.GetOffsetBehindPosition();

                if (agent.Follower.enabled && agent.Follower.remainingDistance <= 1f)
                {
                    playerToEliminate.Stop();
                }
            }

            if (Vector3.Distance(agent.transform.position, agent.FollowTarget.position) > 0.1f)
            {
                agent.transform.position = Vector3.MoveTowards(agent.transform.position, agent.FollowTarget.position, eliminationSpeed * Time.deltaTime);
            }
            else
            {
                if (biteCoroutine == null)
                {
                    biteCoroutine = agent.StartCoroutine(PlayBitingSuccessAnimation(true));
                }
            }
            // Optionally, add extra elimination-specific movement logic here if needed
        }

        public void ExitState(ZombieAgent agent)
        {
            if (agent.Animator != null)
            {
                //   agent.Animator.SetBool("IsEliminating", false);
            }
        }

        private IEnumerator PlayBitingSuccessAnimation(bool value)
        {
            yield return null;
            agent.transform.LookAt(playerToEliminate.transform.position);

            agent.AnimatorPlayer.PlayAnimationWithMode(AnimationHelper.biteAttemptFromBehind + AnimationHelper.Starting);
            playerToEliminate.AnimatorPlayer.PlayAnimationWithMode(AnimationHelper.biteAttemptFromBehind + AnimationHelper.Starting);
            yield return new WaitForSeconds(agent.AnimatorPlayer.GetCurrentClip().length + 0.1f);
            agent.AnimatorPlayer.PlayAnimationWithMode(AnimationHelper.biteAttemptFromBehind + AnimationHelper.Looping);
            playerToEliminate.AnimatorPlayer.PlayAnimationWithMode(AnimationHelper.biteAttemptFromBehind + AnimationHelper.Looping);
            yield return new WaitForSeconds(2f);
            if (value)
            {
                agent.AnimatorPlayer.PlayAnimationWithMode(AnimationHelper.biteAttemptFromBehind + AnimationHelper.Ending);
                playerToEliminate.AnimatorPlayer.PlayAnimationWithMode(AnimationHelper.biteAttemptFromBehind + AnimationHelper.Ending);
            }
            else
            {
                agent.AnimatorPlayer.PlayAnimationWithMode(AnimationHelper.biteAttemptFromBehind + AnimationHelper.ElbowOff);
                playerToEliminate.AnimatorPlayer.PlayAnimationWithMode(AnimationHelper.biteAttemptFromBehind + AnimationHelper.ElbowOff);
            }
            yield return new WaitForSeconds(agent.AnimatorPlayer.GetCurrentClip().length + 0.1f);

            playerToEliminate.AnimatorPlayer.PlayDirectAnimation(AnimationHelper.Death);

            //Then again start running
            agent.StopEliminationBehavior();
        }
    }
}
