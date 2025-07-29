using UnityEngine;
using System.Collections.Generic;

namespace ZombieElimination
{
    /// <summary>
    /// Abstract base class for trigger-based obstacles acting on Players.
    /// </summary>
    public abstract class ObstacleBase : MonoBehaviour
    {
        [Tooltip("Trigger collider object inside obstacle prefab")]
        [SerializeField] protected OnTriggerEvent triggerZone;

        protected bool isTriggerActive = false;

        // List of players currently inside the trigger
        protected readonly List<Player> playersInTrigger = new();

        protected virtual void Awake()
        {
            if (triggerZone != null)
            {
                triggerZone.OnTrigger += OnTriggerHandler;
            }
            else
            {
                ($"ObstacleBase on {gameObject.name} missing assigned triggerZone").LogWarning();
            }
        }

        protected void OnTriggerHandler(Collider other)
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                if (!playersInTrigger.Contains(player))
                {
                    playersInTrigger.Add(player);
                    $"{player.name} entered trigger zone".Log();
                    OnPlayerEntered(player);
                }
            }
        }

        /// <summary>
        /// Called when a Player enters the trigger zone.
        /// Override to implement obstacle-specific logic.
        /// </summary>
        protected abstract void OnPlayerEntered(Player player);

        /// <summary>
        /// Optional: override for OnPlayerExited if needed.
        /// </summary>
        protected virtual void OnPlayerExited(Player player)
        {
            playersInTrigger.Remove(player);
        }
    }
}
