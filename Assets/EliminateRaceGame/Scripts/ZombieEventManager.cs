using System;

namespace ZombieElimination
{
    public class EventManager
    {
        public static EventManager Instance { get; private set; }

        public event Action<Player> OnPlayerEliminated;
        public event Action<Block> OnBlockTriggered;

        public EventManager()
        {
            Instance = this;
        }

        public void CallPlayerEliminated(Player player)
        {
            OnPlayerEliminated?.Invoke(player);
        }

        public void TriggerBlockTriggered(Block block)
        {
            OnBlockTriggered?.Invoke(block);
        }
    }
}
