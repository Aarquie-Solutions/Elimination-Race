using System;
using Pathfinding;
using UnityEngine;

namespace ZombieElimination
{
    public class ZombieAgent : MonoBehaviour
    {
        public Transform FollowTarget { get; private set; }

        [Header("Distance Settings")]
        public float preferredDistance = 10f;
        public float maxDistance = 17f;
        public float distanceTolerance = 2f;

        private AgentSpeedHandler speedHandler;
        private AIDestinationSetter destinationSetter;
        private FollowerEntity followerEntity;

        private IZombieState currentState;

        private NormalChaseState normalChaseState;
        private EliminationState eliminationState;

        public Player PlayerToEliminate => eliminationState.playerToEliminate;

        private AnimatorPlayer animatorPlayer;


        public Animator Animator => animatorPlayer.animator;

        public AnimatorPlayer AnimatorPlayer => animatorPlayer;

        public FollowerEntity FollowerEntity => followerEntity;

        public AgentSpeedHandler SpeedHandler => speedHandler;

        public bool IsEliminating => currentState is EliminationState;

        void Awake()
        {
            animatorPlayer = new AnimatorPlayer(GetComponentInChildren<Animator>().gameObject, false);
            speedHandler = GetComponent<AgentSpeedHandler>();
            if (speedHandler == null)
                Debug.LogError($"{nameof(ZombieAgent)} requires {nameof(AgentSpeedHandler)} component.");

            followerEntity = GetComponent<FollowerEntity>();
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
            followerEntity.maxSpeed = 0;
            followerEntity.rvoSettings.priority = 0;
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
    }
}
