using System.Collections.Generic;

namespace ZombieElimination
{
    public class ZombieChaseElimination : IEliminationCommand
    {
        public void Execute(List<Player> allPlayers, PlayersManager playersManager, EliminationSystem eliminationSystem)
        {
            // Find last player (or use passed index/criteria)
            var lastPlayer = playersManager.GetPlayerWithLowestProgress();
            eliminationSystem.TriggerZombieChase(lastPlayer);
        }
    }
}
