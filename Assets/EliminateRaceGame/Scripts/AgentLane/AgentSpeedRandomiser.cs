using System.Collections;
using System.Collections.Generic;
using AarquieSolutions.Base.Singleton;
using UnityEngine;

namespace EliminateRaceGame
{
    public class AgentSpeedRandomizer : Singleton<AgentSpeedRandomizer>
    {
        public float interval = 2f;
        public float minChange = -1.0f;
        public float maxChange = 1.5f;
        public float minSpeedLimit = 1f;
        public float maxSpeedLimit = 6f;

        private List<AgentController_RVO> agents => AgentController_RVO.allControllers;

        private void Start()
        {
            StartCoroutine(RandomizeSpeedsRoutine());
        }

        private IEnumerator RandomizeSpeedsRoutine()
        {
            yield return new WaitForSeconds(interval);

            while (true)
            {
                yield return new WaitForSeconds(interval);

                if (agents?.Count == 0) continue;

                var selected = agents[Random.Range(0, agents.Count)];
                if (selected == null) continue;

                float originalSpeed = selected.currentSpeed;

                float speedChange = Random.Range(minChange, maxChange);
                float newSpeed = Mathf.Clamp(originalSpeed + speedChange, minSpeedLimit, maxSpeedLimit);

                selected.SetMaxSpeedToReach(newSpeed);

                //($"Adjusted speed of {selected.gameObject.name} to {newSpeed:F2}").Log();
            }
        }
    }
}
