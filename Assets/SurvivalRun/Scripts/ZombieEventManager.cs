using System;

namespace ZombieElimination
{
    public class EventManager
    {
        public static EventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManager();
                }
                return instance;
            }
        }
        private static EventManager instance;

        public event Action<Player> OnPlayerEliminated;
        public event Action<Block> OnBlockTriggered;

        public EventManager()
        {
            instance = this;
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
