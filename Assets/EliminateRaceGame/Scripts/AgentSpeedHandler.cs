using Pathfinding;

namespace ZombieElimination
{
    using UnityEngine;

    public class AgentSpeedHandler : MonoBehaviour, IAgentSpeedHandler
    {
        private IAstarAI follower;
        [Header("Speed settings")]
        private float currentSpeed;
        private float targetSpeed;
        public GameRulesSO gameRules;
        public bool isPlayer = true;

        public float speedLerpRate = 3f;

        public float MinSpeed => isPlayer ? gameRules.playerSpeedRange.x : gameRules.zombieSpeedRange.x;

        public float MaxSpeed => isPlayer ? gameRules.playerSpeedRange.y : gameRules.zombieSpeedRange.y;

        public float CurrentSpeed => currentSpeed;

        public float TargetSpeed => targetSpeed;


        void Awake()
        {
            follower = GetComponent<IAstarAI>();
            currentSpeed = MinSpeed;
            targetSpeed = MinSpeed;
        }

        private void LateUpdate()
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedLerpRate * Time.deltaTime);
            currentSpeed = Mathf.Clamp(currentSpeed, MinSpeed, MaxSpeed);

            if (follower != null)
                follower.maxSpeed = currentSpeed;
        }

        public void SetTargetSpeed(float speed)
        {
            targetSpeed = Mathf.Clamp(speed, MinSpeed, MaxSpeed);
        }
    }
}
