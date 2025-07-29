using UnityEngine;
using System.Collections.Generic;

namespace ZombieElimination
{
    public class TrolleyObstacle : ObstacleBase
    {
        public Transform hitZone;
        public Transform evadePoint;

        protected override void OnPlayerEntered(Player player)
        {
            if (ShouldBeHit(player))
            {
                player.MoveToPosition(hitZone.position, onComplete: player.StartElimination);
            }
            else
            {
                player.MoveToPosition(evadePoint.position, onComplete: player.ResumeNormalMovement);
            }
        }

        private bool ShouldBeHit(Player player)
        {
            // Example: Can use position, random, or other criteria
            return Random.value < 0.4f;
        }
    }
}
