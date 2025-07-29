using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZombieElimination
{
    public class EliminationSystem : MonoBehaviour
    {
        private float eliminationInterval;
        private float timer;
        private List<IEliminationCommand> eliminationCommands;

        private void Start()
        {
            eliminationInterval = ServiceLocator.GameRules.eliminationInterval;
            eliminationCommands = new List<IEliminationCommand>
            {
                new ZombieChaseElimination(),
                new ManholeElimination(),
                new TrolleyElimination(),
                // ...more as needed
            };

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
            // Compose eligible players
            var eligiblePlayers = ServiceLocator.playersManager.Players
                .Where(p => !p.isEliminating && !p.isWinner).ToList();

            //var elimination = eliminationCommands[Random.Range(0, eliminationCommands.Count)];
            var elimination = eliminationCommands[0];

            elimination.Execute(eligiblePlayers, ServiceLocator.playersManager, this);
        }

        public void TriggerZombieChase(Player player)
        {
            ServiceLocator.zombieHordeController?.TriggerZombieChase(player);
        }

        public void TriggerManholeFall(Player player)
        {
            /* ... */
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
            /* ... */
            return null;
        }

        public List<Player> ChoosePlayersForTrolley(List<Player> pool, int count)
        {
            /* ... */
            return null;
        }
    }
}
