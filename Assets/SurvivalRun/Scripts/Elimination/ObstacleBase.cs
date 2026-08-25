using System;
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

        protected readonly List<Player> playersInTrigger = new();

        protected void OnValidate()
        {
            if (!TryGetComponent(out triggerZone))
            {
                triggerZone = GetComponentInChildren<OnTriggerEvent>();
                if (triggerZone == null)
                {
                    triggerZone = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<OnTriggerEvent>();
                    triggerZone.transform.parent = transform;
                    triggerZone.transform.localPosition = Vector3.zero;
                    triggerZone.name = "TriggerZone";
                }
            }
        }

        protected virtual void Awake()
        {
            if (triggerZone != null)
            {
                triggerZone.OnTriggerEnterEvent += OnTriggerEnterEventHandler;
                triggerZone.OnTriggerExitEvent += OnTriggerExitEventHandler;
            }
            else
            {
                ($"ObstacleBase on {gameObject.name} missing assigned triggerZone").LogWarning();
            }
        }

        protected void OnTriggerEnterEventHandler(Collider other)
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                if (!playersInTrigger.Contains(player))
                {
                    playersInTrigger.Add(player);
                    //$"{player.name} entered trigger zone".Log();
                    OnPlayerEntered(player);
                }
            }
        }

        protected void OnTriggerExitEventHandler(Collider other)
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                OnPlayerExited(player);
            }
        }

        protected abstract void OnPlayerEntered(Player player);

        protected virtual void OnPlayerExited(Player player)
        {
            playersInTrigger.Remove(player);
        }
    }
}
