using System.Collections.Generic;
using System.Linq;

namespace ZombieElimination
{
    public class TrolleyElimination : IEliminationCommand
    {
        public void Execute(List<Player> allPlayers, PlayersManager playersManager, EliminationSystem eliminationSystem)
        {
            var victims = eliminationSystem.ChoosePlayersForTrolley(allPlayers, count: 3);
            foreach (var p in victims)
            {
                eliminationSystem.TriggerHitByTrolley(p);
            }
            foreach (var p in allPlayers.Except(victims))
            {
                eliminationSystem.InstructDodgeTrolley(p);
            }
        }
    }
}
