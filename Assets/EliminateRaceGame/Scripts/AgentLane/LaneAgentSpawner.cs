using System.Collections;
using AarquieSolutions.Base.Singleton;
using UnityEngine;

namespace EliminateRaceGame
{
    public class LaneAgentSpawner : Singleton<LaneAgentSpawner>
    {
        public GameObject agentPrefab;
        public Transform spawnCenter;
        public int count = 10;
        public float spacing = 2f; // Offset spacing between agents
        public float timeBetweenSpawns = 0.5f; // Time between agent spawns

        private void Start()
        {
            if (!agentPrefab || !spawnCenter)
            {
                Debug.LogError("Spawner is missing references.");
                return;
            }

            // StartCoroutine(SpawnAgents());
        }

        private IEnumerator SpawnAgents()
        {
            var parent = new GameObject("AgentsRoot");
            parent.transform.position = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                float angle = i * Mathf.PI * 2 / count;
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spacing;
                Vector3 spawnPos = spawnCenter.position + offset;


                GameObject agent = Instantiate(agentPrefab, spawnPos, spawnCenter.rotation);
                agent.name = "Agent Racer_" + i;
                agent.transform.parent = parent.transform;
                // var agentCtrl = agent.GetComponent<AgentController_RVO>();

                // {
                //     agentCtrl.splineContainer = laneManager.GetRandomLane(out int laneIndexToFollow);
                //     agentCtrl.maskIndex = laneIndexToFollow;
                // }
                agent.SetActive(true);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
    }
}
