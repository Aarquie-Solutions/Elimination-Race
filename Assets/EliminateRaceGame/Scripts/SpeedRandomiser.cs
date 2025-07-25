using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZombieElimination
{
    public class SpeedRandomiser : MonoBehaviour
    {
        public float interval = 2f;
        public GameRulesSO gameRules;
        public bool isPlayer;
        public List<IAgentSpeedHandler> iAgents = new List<IAgentSpeedHandler>();

        private void Start()
        {
            iAgents = GetComponentsInChildren<IAgentSpeedHandler>().ToList();
            StartCoroutine(RandomizeSpeedsRoutine());
        }

        private IEnumerator RandomizeSpeedsRoutine()
        {
            var wait = new WaitForSeconds(interval);
            yield return wait;
            while (true)
            {
                yield return wait;

                if (iAgents == null || iAgents.Count == 0)
                    continue;

                var selected = iAgents[Random.Range(0, iAgents.Count)];
                if (selected == null) continue;

                float originalSpeed = selected.CurrentSpeed;
                Vector2 speedChangeFactor = isPlayer ? gameRules.playerSpeedChange : gameRules.zombieSpeedChange;
                float speedChange = Random.Range(speedChangeFactor.x, speedChangeFactor.y);

                float newSpeed = originalSpeed + speedChange;

                selected.SetTargetSpeed(newSpeed);

                //Debug.Log($"Adjusted speed of {((MonoBehaviour)selected).gameObject.name} to {newSpeed:F2}");
            }
        }
    }
}
