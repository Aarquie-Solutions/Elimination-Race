using UnityEngine;
using System.Collections.Generic;

namespace ZombieElimination
{
    /// <summary>
    /// Example ManholeObstacle: Players move to an approach point,
    /// then jump to success or failure points depending on elimination.
    /// </summary>
    public class ManholeObstacle : ObstacleBase
    {
        [Header("Positions")]
        public Transform approachPoint; // Position players move to just before jumping
        public Transform jumpFailPoint; // Where eliminated players jump and fall
        public Transform jumpSuccessPoint; // Where players jump to evade

        [Range(0f, 1f), Tooltip("Chance that a player will fall")]
        public float eliminationChance = 0.5f;

        protected override void OnPlayerEntered(Player player)
        {
            // Determine if player is eliminated (fall) or evades (jump success)
            bool willFall = ShouldEliminate(player);

            // Step 1: Move player to approach position
            player.MoveToPosition(approachPoint.position,
                () =>
                    {
                        // Step 2: Perform jump after arriving at approach point
                        if (willFall)
                        {
                            player.JumpTo(jumpFailPoint.position, jumpHeight: 1.8f, jumpDuration: 0.8f, onComplete: () => { player.StartElimination(); });
                        }
                        else
                        {
                            player.JumpTo(jumpSuccessPoint.position, jumpHeight: 2.2f, jumpDuration: 0.7f, onComplete: () => { player.ResumeNormalMovement(); });
                        }
                    });
        }

        private bool ShouldEliminate(Player player)
        {
            // Example rule: random chance and exclude winner
            return Random.value < eliminationChance && !player.isWinner;
        }
    }
}
