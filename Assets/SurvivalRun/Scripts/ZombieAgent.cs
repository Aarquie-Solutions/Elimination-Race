using System;
using System.Collections;
using Pathfinding;
using UnityEngine;

namespace ZombieElimination
{
    public class ZombieAgent : MonoBehaviour, IJump
    {
        public Transform FollowTarget { get; private set; }

        [Header("Distance Settings")]
        public float preferredDistance = 10f;
        public float maxDistance = 17f;
        public float distanceTolerance = 2f;

        private AgentSpeedHandler speedHandler;
        private AIDestinationSetter destinationSetter;
        private FollowerEntity follower;

        private IZombieState currentState;

        private NormalChaseState normalChaseState;
        private EliminationState eliminationState;

        public Player PlayerToEliminate => eliminationState.playerToEliminate;

        private AnimatorPlayer animatorPlayer;


        public Animator Animator => animatorPlayer.animator;

        public AnimatorPlayer AnimatorPlayer => animatorPlayer;

        public FollowerEntity Follower => follower;

        public AgentSpeedHandler SpeedHandler => speedHandler;

        public bool IsEliminating => currentState is EliminationState;

        void Awake()
        {
            animatorPlayer = new AnimatorPlayer(GetComponentInChildren<Animator>().gameObject, false);
            speedHandler = GetComponent<AgentSpeedHandler>();
            if (speedHandler == null)
                Debug.LogError($"{nameof(ZombieAgent)} requires {nameof(AgentSpeedHandler)} component.");

            follower = GetComponent<FollowerEntity>();
            if (!transform.TryGetComponent<AIDestinationSetter>(out destinationSetter))
            {
                destinationSetter = gameObject.AddComponent<AIDestinationSetter>();
            }

            normalChaseState = new NormalChaseState();
        }

        private void Start()
        {
            float eliminationSpeed = speedHandler.maxSpeed * 1.2f;
            eliminationState = new EliminationState(eliminationSpeed);
            SetState(normalChaseState);
        }

        void Update()
        {
            currentState?.UpdateState(this);
            if (!IsEliminating)
                speedHandler.UpdateSpeed();
        }

        public void SetFollowTarget(Transform target)
        {
            FollowTarget = target;
            destinationSetter.target = FollowTarget;
        }

        public void SetState(IZombieState newState)
        {
            if (currentState != null)
                currentState.ExitState(this);

            currentState = newState;

            if (currentState != null)
                currentState.EnterState(this);
        }

        public void StartEliminationBehavior()
        {
            SetState(eliminationState);
        }

        public void StopFollower()
        {
            follower.maxSpeed = 0;
            follower.rvoSettings.priority = 0;
        }

        public void StopEliminationBehavior()
        {
            SetState(normalChaseState);
        }

        void OnDrawGizmos()
        {
            if (FollowTarget == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(FollowTarget.position, 0.5f);
            Gizmos.DrawLine(transform.position + Vector3.up, FollowTarget.position + Vector3.up);
        }

        public void SetFollowPosition(Vector3 position)
        {
            if (!IsEliminating)
                FollowTarget.position = position;
        }

        public void Eliminate(Player targetPlayer)
        {
            eliminationState.playerToEliminate = targetPlayer;
        }

        public void Jump(Vector3 destination, float jumpHeight, float jumpDuration, Action onComplete = null)
        {
            StartCoroutine(JumpArcCoroutine(transform.position, destination, jumpHeight, jumpDuration, onComplete));
        }

        private IEnumerator JumpArcCoroutine(Vector3 start, Vector3 end, float height, float duration, Action onComplete = null)
        {
            if (follower != null)
                follower.enabled = false;

            float elapsed = 0;
            //overriding duration
            float distance = start.DistanceXZ(end);
            duration = distance / speedHandler.CurrentSpeed;
            
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                Vector3 pos = Vector3.Lerp(start, end, t);
                pos.y += height * 4 * (t - t * t);

                transform.position = pos;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;

            // landing animation or fx

            if (follower != null)
                follower.enabled = true;

            onComplete?.Invoke();
        }
    }
}
