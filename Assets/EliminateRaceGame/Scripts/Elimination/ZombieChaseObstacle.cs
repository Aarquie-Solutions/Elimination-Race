using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ZombieElimination
{
    public class ZombieChaseObstacle : ObstacleBase
    {
        // You can optionally map this obstacle to specific zombies or have custom parameters if needed

        protected override void OnPlayerEntered(Player player)
        {
            if (isTriggerActive)
            {
                return;
            }
            isTriggerActive = true;
            var targetPlayer = ServiceLocator.playersManager.GetPlayerWithLowestProgress();
            $"{targetPlayer.name} Found trigger zone".Log();

            var zombieHordeController = ServiceLocator.zombieHordeController;
            if (zombieHordeController != null)
            {
                zombieHordeController.TriggerZombieChase(targetPlayer);
                targetPlayer.StartElimination();
            }
        }
    }
}
