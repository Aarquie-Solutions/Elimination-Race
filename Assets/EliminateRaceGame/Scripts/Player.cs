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
        /// Jump to a destination following a parabolic arc.
        /// Disables follower during jump and re-enables after landing.
        /// </summary>
        public void JumpTo(Vector3 destination, float jumpHeight, float jumpDuration, Action onComplete = null)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            StartCoroutine(JumpArcCoroutine(transform.position, destination, jumpHeight, jumpDuration, onComplete));
        }

        private IEnumerator JumpArcCoroutine(Vector3 start, Vector3 end, float height, float duration, Action onComplete)
        {
            if (follower != null)
                follower.enabled = false;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                Vector3 pos = Vector3.Lerp(start, end, t);
                pos.y += height * 4f * (t - t * t); // Parabola formula
                transform.position = pos;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;

            // Optionally: play landing animation or effects

            if (follower != null && !isEliminating)
                follower.enabled = true;

            onComplete?.Invoke();
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
    }
}
