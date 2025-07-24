using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace EliminateRaceGame
{
    public class FallingTreeTrigger : ObstacleTrigger
    {
        public float fallDuration = 1f;
        private float fallTimer;
        [SerializeField] private Transform startRotation;
        [SerializeField] private Transform endRotation;
        public Transform tree;
        public float radius = 10f;

        public OnTriggerEvent onTreeHit;

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
            EliminationTag = EliminationTag.FallingTree;
            RetrieveLanes();
            onTreeHit.OnTrigger += TreeHit;
            MaxAgentsToEliminate = 1;
        }

        private void TreeHit(Collider obj)
        {
            if (obj.TryGetComponent(out AgentController_RVO agent))
            {
                agent.Eliminate();
            }
        }

        private bool treeCanFall;

        private void Update()
        {
            if (!IsSet)
            {
                return;
            }
            if (!treeCanFall)
            {
                if (agentsToEliminate is { Count: > 0 })
                {
                    var nearestAgent = agentsToEliminate.Find(x => x != null && Vector3.Distance(x.transform.position, tree.transform.position) <= radius);
                    if (nearestAgent != null)
                    {
                        treeCanFall = true;
                    }
                }
                return;
            }
            if (fallTimer >= fallDuration)
            {
                return;
            }

            fallTimer += Time.deltaTime;
            DOTween.To(() => tree.rotation, (x) => tree.rotation = x, endRotation.rotation.eulerAngles, fallDuration).SetEase(Ease.InCirc);
        }


        private void RetrieveLanes()
        {
            int nearestLaneIndex = LaneManager.Instance.GetLaneIndex(tree.position);

            affectedLanes ??= new();
            if (nearestLaneIndex == 0)
            {
                affectedLanes = new List<int> { 0, 1, 2 };
            }
            else
            {
                affectedLanes = new List<int> { 4, 3, 2 };
            }

            SetSafeLanes();
        }


        protected override void OnAgentTriggered(AgentController_RVO agent)
        {
            if (!IsSet)
            {
                $"Tree falling Trigger Activated".Log();
                IsSet = true;
                AssignAgentsToEliminate();
                SetEliminateProperties();
            }
            agent.HandleForceSwitchLanes(eliminateProperties);
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
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(tree.position, radius);
        }
    }
}
