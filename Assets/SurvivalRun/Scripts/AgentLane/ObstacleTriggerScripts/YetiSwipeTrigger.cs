using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace EliminateRaceGame
{
    public class YetiSwipeTrigger : ObstacleTrigger
    {
        public float swipeRadius = 9f;

        public OnTriggerEvent onYetiSwipeTrigger;
        public GameObject yeti;


        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }


        protected override void Initialise()
        {
            onYetiSwipeTrigger.OnTriggerEnterEvent += YetiSwipeTriggerEnterEventActions;
            EliminationTag = EliminationTag.YetiSwipe;
            MaxAgentsToEliminate = Random.Range(1, 3);
            SetSafeLanes();
        }

        private void YetiSwipeTriggerEnterEventActions(Collider obj)
        {
            if (obj.TryGetComponent(out AgentController_RVO agent))
            {
                agent.Eliminate();
                agentsToEliminate.Remove(agent);
            }
        }

        private void Update()
        {
        }

        private void LateUpdate()
        {
            if (!IsSet)
            {
                yeti.SetActive(false);
                return;
            }
            if (agentsToEliminate is { Count: > 0 })
            {
                yeti.SetActive(agentsToEliminate.Find(x => x != null && Vector3.Distance(x.transform.position, yeti.transform.position) <= swipeRadius) != null);
            }
            else
            {
                yeti.SetActive(false);
            }
        }

        protected override void OnAgentTriggered(AgentController_RVO agent)
        {
            if (!IsSet)
            {
                $"Yeti Swipe Trigger Activated".Log();
                IsSet = true;
                AssignAgentsToEliminate();
                SetEliminateProperties();
            }
            agent.HandleForceSwitchLanes(eliminateProperties);

            // if (agent != null && !agent.isWinner && edgeLanes.Contains(agent.currentLaneIndex) &&
            //     Vector3.Distance(transform.position, agent.transform.position) <= swipeRadius)
            // {
            //     Debug.Log($"{agent.gameObject.name} swiped by yeti!");
            //     agent.Eliminate();
            // }
        }


        protected override bool CanActivateCondition()
        {
            return false;
        }

        protected override void ActivateObstacle()
        {
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(yeti.transform.position, swipeRadius);
        }
    }
}
