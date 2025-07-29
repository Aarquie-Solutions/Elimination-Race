using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace ZombieElimination
{
    public partial class Player : MonoBehaviour
    {
        public float thresholdDistance = 6;
        public bool isWinner;

        private FollowerEntity follower;


        private AgentSpeedHandler speedHandler;
        public bool isEliminating;

        private AnimatorPlayer animatorPlayer;

        public AnimatorPlayer AnimatorPlayer => animatorPlayer;

        void Awake()
        {
            follower = GetComponent<FollowerEntity>();
            speedHandler = GetComponent<AgentSpeedHandler>();
            animatorPlayer = new AnimatorPlayer(GetComponentInChildren<Animator>().gameObject, true);
            if (speedHandler != null)
            {
                speedHandler.isPlayer = true;
            }
        }


        void Update()
        {
            if (follower == null)
            {
                return;
            }
            if (isEliminating)
            {
                return;
            }
            speedHandler.UpdateSpeed();
        }


        private void SetDestination(Vector3 position)
        {
            // currentDestination = position;
            follower.destination = position;
        }

        public void StartElimination()
        {
            isEliminating = true;
            follower.maxSpeed = speedHandler.minSpeed;
            EventManager.Instance.CallPlayerEliminated(this);
        }

        public void EliminationTrigger()
        {
            follower.maxSpeed = 0;
            follower.enabled = false;
        }

        public Vector3 GetPlayerBehindOffsetPosition()
        {
            return transform.position - transform.forward;
        }
    }
}
