using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieElimination
{
    public class ZombieHordeController : MonoBehaviour
    {
        [Header("Zombies")]
        public List<ZombieAgent> zombieAgents = new();

        [Header("Follow & Elimination Settings")]
        public float followDistance = 10f;
        public float updateInterval = 0.5f;
        public GameRulesSO gameRules;
        // public float eliminationInterval = 15f; // Seconds between eliminations
        public float eliminationDuration = 5f; // How long the elimination behavior lasts

        public Player currentLastPlayer;
        // public Transform currentTarget;

        private float timer;

        private void Start()
        {
            ServiceLocator.zombieHordeController = this;

            zombieAgents = GetComponentsInChildren<ZombieAgent>().ToList();

            // currentTarget = new GameObject("ZombiesTarget").transform;

            for (int i = 0; i < zombieAgents.Count; i++)
            {
                ZombieAgent zombieAgent = zombieAgents[i];
                zombieAgent.SetFollowTarget(new GameObject("ZombieTarget_" + i).transform);
                zombieAgent.StopEliminationBehavior();
            }

            StartCoroutine(EliminationRoutine());
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                timer = 0f;

                var lastPlayer = ServiceLocator.playersManager.GetPlayerWithLowestProgress();
                if (lastPlayer == null)
                    return;

                currentLastPlayer = lastPlayer;
                foreach (ZombieAgent agent in zombieAgents)
                {
                    if (agent.IsEliminating)
                    {
                        continue;
                    }
                    if (agent.FollowTarget.position == Vector3.zero)
                    {
                        agent.FollowTarget.position = currentLastPlayer.transform.position;
                    }

                    if (Vector3.Distance(currentLastPlayer.transform.position, agent.FollowTarget.position) > followDistance)
                    {
                        agent.SetFollowPosition(currentLastPlayer.transform.position);
                    }
                }
            }
        }

        private IEnumerator EliminationRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(gameRules.eliminationInterval);

                currentLastPlayer = ServiceLocator.playersManager.GetPlayerWithLowestProgress();
                if (currentLastPlayer == null || zombieAgents.Count == 0)
                    continue;

                // Pick a zombie not already eliminating and not targeting last player
                var candidates = zombieAgents.Where(z => !z.IsEliminating &&
                                                         z.PlayerToEliminate != currentLastPlayer).ToList();

                if (candidates.Count == 0)
                    continue;

                var eliminator = candidates[Random.Range(0, candidates.Count)];

                $"Zombie {eliminator.name} is eliminating player {currentLastPlayer.name}".Log();

                // eliminator.SetFollowTarget(currentLastPlayer);
                eliminator.Eliminate(currentLastPlayer);
                eliminator.StartEliminationBehavior();

                yield return new WaitForSeconds(eliminationDuration);

                // // Return zombie to group target and stop elimination behavior
                // eliminator.SetFollowTarget(currentTarget);
                // eliminator.StopEliminationBehavior();
            }
        }
    }
}
