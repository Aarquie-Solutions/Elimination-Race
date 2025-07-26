using Pathfinding; // For IAstarAI interface
using UnityEngine;

namespace ZombieElimination
{
    public class AgentSpeedHandler : MonoBehaviour, IAgentSpeedHandler
    {
        // Reference to the agent's movement/pathfinding interface
        private IAstarAI follower;

        [Header("Game Rules (Assign in Inspector)")]
        [Tooltip("Central config for agent speed (min/max per type)")]
        public GameRulesSO gameRules;

        [Header("Role")]
        [Tooltip("Set true if this is a Player; false for Zombie/NPC")]
        public bool isPlayer = true;

        [Header("Speed Lerp Settings")]
        [Tooltip("How quickly current speed approaches target speed (units/sec)")]
        public float speedLerpRate = 3f;

        private float currentSpeed;
        private float targetSpeed;

        /// <summary>
        /// Returns current min speed for this agent based on type/rules
        /// </summary>
        public float minSpeed =>
            gameRules != null
                ? (isPlayer ? gameRules.playerSpeedRange.x : gameRules.zombieSpeedRange.x)
                : 1f; // fallback default

        /// <summary>
        /// Returns current max speed for this agent based on type/rules
        /// </summary>
        public float maxSpeed =>
            gameRules != null
                ? (isPlayer ? gameRules.playerSpeedRange.y : gameRules.zombieSpeedRange.y)
                : 10f; // fallback default

        public float CurrentSpeed => currentSpeed;

        public float TargetSpeed => targetSpeed;

        private void Awake()
        {
            follower = GetComponent<IAstarAI>();
            currentSpeed = minSpeed;
            targetSpeed = minSpeed;
        }

        public void UpdateSpeed()
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedLerpRate * Time.deltaTime);

            currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

            if (follower != null)
                follower.maxSpeed = currentSpeed;
        }

        public void SetTargetSpeed(float speed)
        {
            targetSpeed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }
    }
}
