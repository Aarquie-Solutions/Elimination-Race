namespace ZombieElimination
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GameRules", menuName = "Game/GameRules", order = 0)]
    public class GameRulesSO : ScriptableObject
    {
        [Header("Speed Settings")]
        public Vector2 playerSpeedRange = new Vector2(4f, 6f); //min(X), max(Y)

        public Vector2 zombieSpeedRange = new Vector2(3f, 5f); //min(X), max(Y)
        
        public Vector2 playerSpeedChange = new Vector2(0.3f, 0.5f); //min(X), max(Y)
        public Vector2 zombieSpeedChange = new Vector2(0.3f, 0.6f); //min(X), max(Y)

        [Header("Game Flow")]
        public int winningScore = 10;
        public float roundTimeLimit = 120f;

        [Header("Elimination")]
        public bool enableElimination = true;
        public int eliminationInterval = 15; //  eliminate slowest player

        // Extend with more rule categories as needed!
    }
}
