using System.Collections.Generic;
using System.Linq;

namespace ZombieElimination
{
    public class ManholeElimination : IEliminationCommand
    {
        public void Execute(List<Player> allPlayers, PlayersManager playersManager, EliminationSystem eliminationSystem)
        {
            var candidates = eliminationSystem.ChoosePlayersForManhole(allPlayers, count: 2);
            foreach (var p in candidates)
            {
                eliminationSystem.TriggerManholeFall(p);
            }
            // Evade logic for others
            foreach (var p in allPlayers.Except(candidates))
            {
                eliminationSystem.InstructJumpOverManhole(p);
            }
        }
    }

}
