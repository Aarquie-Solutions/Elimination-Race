using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace ZombieElimination
{
    public class TrolleyObstacle : ObstacleBase
    {
        public Transform hitPoint;
        public TrolleyMover trolleyMover;
        public OnTriggerEvent trolleyHitEvent;
        public float timeToReach = 4f;

        protected override void Awake()
        {
            base.Awake();
            trolleyHitEvent.OnTriggerEnterEvent += OnTrolleyHit;
        }

        private void OnTrolleyHit(Collider obj)
        {
            if (obj.TryGetComponent(out Player player))
            {
                StartCoroutine(PlayerHit(player));
                
            }
        }

        private IEnumerator PlayerHit(Player player)
        {
            $"Player {player.name} was hit by trolley".Log();
            player.StartElimination();
            player.Stop();
            player.EnableRagdoll();
            yield return new WaitForSecondsRealtime(1f);
            player.Die();
        }

        protected override void OnPlayerEntered(Player player)
        {
            if (isTriggerActive)
            {
                return;
            }
            trolleyMover.MoveToInTime(hitPoint.position, timeToReach);
            isTriggerActive = true;
            // if (ShouldBeHit(player))
            // {
            //     player.MoveToPosition(hitZone.position, onComplete: player.StartElimination);
            // }
            // else
            // {
            //     player.MoveToPosition(evadePoint.position, onComplete: player.ResumeNormalMovement);
            // }
        }
    }
}
