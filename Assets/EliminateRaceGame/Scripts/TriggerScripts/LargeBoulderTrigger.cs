using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace EliminateRaceGame
{
    public class LargeBoulderTrigger : ObstacleTrigger
    {
        public float speed = 3f;
        public float spinSpeed;
        public SplineContainer spline;
        public Transform spawnPoint;

        public Transform boulderPivot;
        private Transform actualBoulder;
        public OnTriggerEvent boulderTrigger;

        [Header("Debug")]
        private Vector3 actualBoulderFixedPos;
        private float distanceTravelFrom;
        private float maxDistanceToTravel;
        private float t;
        private float totalLength;

        private void BoulderHit(Collider obj)
        {
            if (obj.TryGetComponent(out AgentController_RVO agent))
            {
                (agent).Eliminate();
            }
        }



        private void Update()
        {
            if (spline == null) return;
            if (!boulderPivot.gameObject.activeSelf)
            {
                return;
            }
            distanceTravelFrom = (distanceTravelFrom - (Time.deltaTime * speed));
            distanceTravelFrom = Mathf.Clamp(distanceTravelFrom, 0f, maxDistanceToTravel);
            t = Mathf.Clamp01(distanceTravelFrom / totalLength);

            boulderPivot.position = spline.Spline.EvaluatePosition(t);
            boulderPivot.forward = spline.Spline.EvaluateTangent(t);

            actualBoulder.Rotate(Vector3.right, Time.deltaTime * -spinSpeed);
        }


        private void LateUpdate()
        {
            actualBoulder.localPosition = actualBoulderFixedPos;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);

            Gizmos.color = Color.magenta;
            if (spline == null)
            {
                $"No Spline Assigned".LogError();
            }
            else
            {
                boulderPivot.position = spline.Spline.EvaluatePosition(t);
                Gizmos.DrawWireSphere(boulderPivot.position, 0.5f);
            }

            Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
        }

        private void OnValidate()
        {
            var col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            SplineUtility.GetNearestPoint(spline.Spline, spawnPoint.position, out var pos, out var t1);
            maxDistanceToTravel = t1 * totalLength;
            distanceTravelFrom = Mathf.Clamp(distanceTravelFrom, 0f, maxDistanceToTravel);
            totalLength = spline.Spline.CalculateLength(spline.transform.localToWorldMatrix);
            t = Mathf.Clamp01(distanceTravelFrom / totalLength);
        }

        protected override void Initialise()
        {
            actualBoulder = boulderPivot.GetChild(0);
            actualBoulderFixedPos = actualBoulder.localPosition;
            totalLength = spline.Spline.CalculateLength(spline.transform.localToWorldMatrix);
            SplineUtility.GetNearestPoint(spline.Spline, spawnPoint.position, out var pos, out var t1);
            maxDistanceToTravel = t1 * totalLength;
            distanceTravelFrom = maxDistanceToTravel;

            AdjustSpinSpeed();

            boulderTrigger.OnTrigger += BoulderHit;
            boulderPivot.gameObject.SetActive(false);
            EliminationTag = EliminationTag.LargeBoulder;
            MaxAgentsToEliminate = Random.Range(1, 4);
            RetrieveLanes();
        }

        private void AdjustSpinSpeed()
        {
            var radius = actualBoulder.lossyScale.x * 0.5f;
            var angularVelocity = speed / radius;
            spinSpeed = angularVelocity * Mathf.Rad2Deg;
        }

        private void RetrieveLanes()
        {
            int originalLaneIndex = LaneManager.Instance.GetLaneIndex(spline);
            int left = Mathf.Clamp(originalLaneIndex - 1, 0, LaneManager.Instance.LaneCount);
            int right = Mathf.Clamp(originalLaneIndex + 1, 0, LaneManager.Instance.LaneCount);

            affectedLanes ??= new();
            foreach (var laneIndex in new int[] { left, originalLaneIndex, right })
            {
                if (affectedLanes.Contains(laneIndex)) continue;
                affectedLanes.Add(laneIndex);
            }
            SetSafeLanes();
        }


        List<AgentController_RVO> agents = new List<AgentController_RVO>();


        protected override void OnAgentTriggered(AgentController_RVO agent)
        {
            {
                if (!IsSet)
                {
                    IsSet = true;
                    AssignAgentsToEliminate();
                    SetEliminateProperties();
                }
                agents ??= new();
                agents.Add(agent);

                agent.HandleForceSwitchLanes(eliminateProperties);

                //if (CanActivateCondition())
                {
                    // Start Boulder Roll
                    ActivateObstacle();
                }
            }
        }

        protected override bool CanActivateCondition()
        {
            if (agents.Count > AgentController_RVO.allControllers.Count / 2)
            {
                return true;
            }
            return false;
        }

        protected override void ActivateObstacle()
        {
            if (boulderPivot.gameObject.activeSelf)
            {
                return;
            }
            boulderPivot.gameObject.SetActive(true);
        }
    }
}
