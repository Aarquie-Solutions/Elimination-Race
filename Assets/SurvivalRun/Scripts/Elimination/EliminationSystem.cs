using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZombieElimination
{
    public class EliminationSystem : MonoBehaviour
    {
        private float eliminationInterval;
        private float timer;

        private void Start()
        {
            eliminationInterval = ServiceLocator.GameRules.eliminationInterval;
            timer = 0f;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= eliminationInterval)
            {
                timer = 0;
                TriggerRandomElimination();
            }
        }

        private void TriggerRandomElimination()
        {
            var eligiblePlayers = ServiceLocator.playersManager.Players
                .Where(p => !p.isEliminating && !p.isWinner).ToList();

        }

        public void TriggerZombieChase(Player player)
        {
            player.StartElimination();
            ServiceLocator.zombieHordeController?.TriggerZombieChase(player);
        }

        public void TriggerManholeFall(Player player)
        {
           
        }

        public void InstructJumpOverManhole(Player player)
        {
            /* ... */
        }

        public void TriggerHitByTrolley(Player player)
        {
            /* ... */
        }

        public void InstructDodgeTrolley(Player player)
        {
            /* ... */
        }

        public List<Player> ChoosePlayersForManhole(List<Player> pool, int count)
        {
            var eligiblePlayers = ServiceLocator.playersManager.Players
                .Where(p => !p.isEliminating && !p.isWinner).Take(Random.Range(1, count + 1)).ToList();
            return eligiblePlayers;
        }

        public List<Player> ChoosePlayersForTrolley(List<Player> pool, int count)
        {
            /* ... */
            return null;
        }
    }
}
