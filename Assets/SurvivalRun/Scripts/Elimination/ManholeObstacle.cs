using UnityEngine;
using System.Collections.Generic;

namespace ZombieElimination
{
    public class ManholeObstacle : ObstacleBase
    {
        public OnTriggerEvent approachZone;
        [Header("Positions")]
        public Transform approachPoint;
        public Transform jumpFailPoint;
        public Transform jumpSuccessPoint;

        public int playersToEliminate = 1;

        public List<Player> playersToEliminateList = new List<Player>();

        protected override void Awake()
        {
            base.Awake();
            approachZone.OnTriggerEnterEvent += OnPlayerToJumpTriggerEnterEvent;
            playersToEliminateList = new List<Player>();
        }

        private void OnPlayerToJumpTriggerEnterEvent(Collider obj)
        {
            if (isTriggerActive)
            {
                return;
            }
            isTriggerActive = true;
        }

        protected override void OnPlayerEntered(Player player)
        {
            bool willFall = false;
            if (!playersToEliminateList.Contains(player) && playersToEliminateList.Count < playersToEliminate)
            {
                playersToEliminateList.Add(player);
                willFall = true;
                $"Player {player.name} Will Fall".Log();
            }

            if (willFall)
            {
                // Step 1: Move player to approach position
                player.MoveToPosition(approachPoint.position,
                    () =>
                        {
                            player.Jump(jumpFailPoint.position,
                                jumpHeight: 1.8f,
                                jumpDuration: 0.8f,
                                onComplete: () =>
                                    {
                                        player.StartElimination();
                                        player.Stop();
                                        player.Fall();
                                    });
                            // else
                            // {
                            //     player.JumpTo(jumpSuccessPoint.position, jumpHeight: 2.2f, jumpDuration: 0.8f, onComplete: player.ResumeNormalMovement);
                            // }
                            // });
                        });
            }
        }
    }
}
