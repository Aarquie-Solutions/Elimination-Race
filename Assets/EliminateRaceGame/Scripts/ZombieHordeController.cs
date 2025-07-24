using System.Linq;
using System.Collections.Generic;
using UnityEngine;


namespace ZombieElimination
{
    public class ZombieHordeController : MonoBehaviour
    {
        [Header("Zombies")]
        public List<ZombieAgent> zombieAgents = new();

        public float followDistance = 10f;
        public float updateInterval = 0.5f;

        public Transform currentLastPlayer;

        public Transform currentTarget;

        private float timer;

        void Start()
        {
            zombieAgents = GetComponentsInChildren<ZombieAgent>().ToList();
            currentTarget = new GameObject("ZombiesTarget").transform;

            foreach (ZombieAgent zombieAgent in zombieAgents)
            {
                zombieAgent.SetFollowTarget(currentTarget);
            }
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                timer = 0f;
                currentLastPlayer = PlayersManager.Instance.GetPlayerWithLowestProgress().transform;
                if (currentTarget.position == Vector3.zero)
                {
                    currentTarget.position = currentLastPlayer.position;
                }
                if (Vector3.Distance(currentLastPlayer.position, currentTarget.position) > followDistance)
                {
                    currentTarget.position = currentLastPlayer.position;
                }
                // UpdateZombieTargets();
            }
        }

        // void UpdateZombieTargets()
        // {
        //     foreach (ZombieAgent zombieAgent in zombieAgents)
        //     {
        //         zombieAgent.SetFollowTarget(currentTarget);
        //     }
        // }
    }
}
