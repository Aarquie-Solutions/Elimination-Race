using System.Collections.Generic;

namespace ZombieElimination
{
    public interface IEliminationCommand
    {
        void Execute(List<Player> allPlayers, PlayersManager playersManager, EliminationSystem eliminationSystem);
    }
}
