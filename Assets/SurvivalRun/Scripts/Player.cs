using System;
using System.Collections;
using UnityEngine;
using Pathfinding; // for FollowerEntity

namespace ZombieElimination
{
    public partial class Player : MonoBehaviour
    {
        // Player state flags
        public bool isWinner;
        public bool isEliminating = false;

        private FollowerEntity follower;
        private AIDestinationSetter aiDestinationSetter;
        private AgentSpeedHandler speedHandler;
        private AnimatorPlayer animatorPlayer;

        public AnimatorPlayer AnimatorPlayer => animatorPlayer;

        private Rigidbody rb;
        private Coroutine moveCoroutine;

        private RagdollBody ragdollBody;

        private void Awake()
        {
            follower = GetComponent<FollowerEntity>();
            aiDestinationSetter = GetComponent<AIDestinationSetter>();
            speedHandler = GetComponent<AgentSpeedHandler>();
            animatorPlayer = new AnimatorPlayer(GetComponentInChildren<Animator>().gameObject, true);
            if (speedHandler != null)
                speedHandler.isPlayer = true;
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;

            ragdollBody = GetComponentInChildren<RagdollBody>(true);
        }

        private void Update()
        {
            if (follower == null || isEliminating)
                return;

            speedHandler.UpdateSpeed();
        }

        /// <summary>
        /// Sets a destination for the pathfinder.
        /// </summary>
        public void SetDestination(Vector3 position)
        {
            if (follower != null)
                follower.destination = position;
        }

        /// <summary>
        /// Move the player to a target position, then call onComplete callback.
        /// Uses the pathfinding system (follower) for movement.
        /// </summary>
        public void MoveToPosition(Vector3 targetPosition, Action onComplete = null, float arrivalThreshold = 0.2f)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(MoveToPositionCoroutine(targetPosition, onComplete, arrivalThreshold));
        }

        private IEnumerator MoveToPositionCoroutine(Vector3 target, Action onComplete, float threshold)
        {
            if (follower != null)
                follower.enabled = true;
            aiDestinationSetter.enabled = false;
            SetDestination(target);

            // Wait until within threshold distance to target
            while (transform.position.DistanceXZ(target) > threshold)
            {
                yield return null;
            }

            onComplete?.Invoke();
            aiDestinationSetter.enabled = true;
            moveCoroutine = null;
        }


        /// <summary>
        /// Starts elimination state for player: stops movement and triggers elimination logic.
        /// </summary>
        public void StartElimination()
        {
            isEliminating = true;
            if (follower != null)
                follower.maxSpeed = speedHandler != null ? speedHandler.minSpeed : 0;

            EventManager.Instance.CallPlayerEliminated(this);
        }

        public void Fall()
        {
            rb.isKinematic = false;
        }

        /// <summary>
        /// Stops all player movement immediately.
        /// </summary>
        public void Stop()
        {
            if (follower != null)
            {
                follower.maxSpeed = 0;
                follower.enabled = false;
                aiDestinationSetter.enabled = false;
            }
        }

        /// <summary>
        /// Resumes normal player movement.
        /// </summary>
        public void ResumeNormalMovement()
        {
            isEliminating = false;
            if (follower != null && speedHandler != null)
            {
                follower.maxSpeed = speedHandler.maxSpeed;
                follower.enabled = true;
            }
        }

        public Vector3 GetOffsetBehindPosition()
        {
            return transform.position - transform.forward;
        }

        public void Die()
        {
            animatorPlayer.Die();
        }

        public void EnableRagdoll()
        {
            animatorPlayer.animator.gameObject.SetActive(false);
            ragdollBody.ApplyForce(Vector3.up);
        }
    }
}
