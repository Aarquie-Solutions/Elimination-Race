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
        public GameRulesSO gameRules => ServiceLocator.GameRules;
        public float eliminationDuration = 5f; // How long the elimination behavior lasts

        public Player currentLastPlayer;

        private float timer;

        private void Start()
        {
            ServiceLocator.zombieHordeController = this;

            zombieAgents = GetComponentsInChildren<ZombieAgent>().ToList();


            for (int i = 0; i < zombieAgents.Count; i++)
            {
                ZombieAgent zombieAgent = zombieAgents[i];
                zombieAgent.SetFollowTarget(new GameObject("ZombieTarget_" + i).transform);
                zombieAgent.StopEliminationBehavior();
            }

            // StartCoroutine(EliminationRoutine());
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= updateInterval)
            {
                timer = 0f;

                UpdateFollowLogic();
            }
        }

        private void UpdateFollowLogic()
        {
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

        /// <summary>
        /// Public method for EliminationSystem to trigger a zombie elimination chase.
        /// </summary>
        public void TriggerZombieChase(Player targetPlayer)
        {
            // Pick a zombie not already eliminating and not targeting this player
            var candidates = zombieAgents.Where(z => !z.IsEliminating &&
                                                     z.PlayerToEliminate != targetPlayer).ToList();
            if (candidates.Count == 0)
                return;

            var eliminator = candidates[Random.Range(0, candidates.Count)];

            $"Zombie {eliminator.name} is chasing player {targetPlayer.name}".Log();

            eliminator.Eliminate(targetPlayer);
            eliminator.StartEliminationBehavior();

            // Optional: Start coroutine to reset after eliminationDuration if you want "zombie returns after eat"
            StartCoroutine(ResetZombieAfterDelay(eliminator, eliminationDuration));
        }

        private IEnumerator ResetZombieAfterDelay(ZombieAgent eliminator, float delay)
        {
            yield return new WaitForSeconds(delay);
            eliminator.StopEliminationBehavior();
            // Optionally return it to a default position or target here
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
