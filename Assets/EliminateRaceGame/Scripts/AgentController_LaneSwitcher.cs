using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace EliminateRaceGame
{
    public partial class AgentController_RVO
    {
        [Header("LaneSwitching")]
        public int currentLaneIndex = 0;
        public float laneSwitchCooldown = 2f;
        public float lastLaneSwitchTime = -10f;

        [Header("Predictive Lookahead")]
        [SerializeField] private float predictiveLookaheadDistance = 5f;

        [Header("Overtaking Priority")]
        public float basePriority = 0.5f; // Static base priority
        public float speedPriorityWeight = 0.5f; // Priority factor based on speed
        public float minSpeedToOvertake = 0.1f;
        public float switchingPriority = 1f;
        private bool isSwitchingLane;
        [Header("Debug")]
        public int forceSwitchLaneIndex = -1;


        private void MoveCloserToForceTargetlane()
        {
            if (Time.time <= lastLaneSwitchTime + laneSwitchCooldown)
            {
                return;
            }
            if (currentLaneIndex == forceSwitchLaneIndex)
            {
                return;
            }
            int targetLane = Utils.MoveTowards(currentLaneIndex, forceSwitchLaneIndex);
            SwitchToLane(targetLane);
        }

        public void HandleForceSwitchLanes(EliminateTriggerProperties eliminateProperties)
        {
            currentEliminationProperties = eliminateProperties;

            if (eliminateProperties != null)
            {
                var currentObstacleProps = eliminateProperties.properties; // ObstacleSpawner.CurrentObstacleProps;
                if (eliminateProperties.agentsToEliminate.Contains(AgentID))
                {
                    eliminationTag = currentObstacleProps.EliminationTag;
                }
                else
                {
                    eliminationTag = EliminationTag.MoveToSafe;
                }

                if (WillDie)
                {
                    forceSwitchLaneIndex = currentObstacleProps.AffectedLanes.GetRandomElement();
                    $"{gameObject.name} will die - forcelane -> {forceSwitchLaneIndex}".Log();
                }
                else
                {
                    forceSwitchLaneIndex = currentObstacleProps.SafeLanes.GetRandomElement();
                    $"{gameObject.name} will not die - forcelane -> {forceSwitchLaneIndex}".Log();
                }
            }
            else
            {
                eliminationTag = EliminationTag.None;
                forceSwitchLaneIndex = -1;
            }
        }

        private void TrySwitchLanes()
        {
            if (Time.time <= lastLaneSwitchTime + laneSwitchCooldown)
            {
                return;
            }
            int[] offsets = Random.value > 0.5f ? new int[] { -1, 1 } : new int[] { 1, -1 };

            foreach (int offset in offsets)
            {
                int newIndex = currentLaneIndex + offset;
                if (newIndex < 0 || newIndex >= LaneManager.Instance.LaneCount) continue;
                if (!IsLaneBlocked(newIndex))
                {
                    SwitchToLane(newIndex);
                    lastLaneSwitchTime = Time.time;
                    break;
                }
            }
        }

        private int GetNearestLaneFromSafeOrAffected(List<int> lanes)
        {
            int minDistance = int.MaxValue;
            List<int> nearestLanes = new();

            foreach (int lane in lanes)
            {
                int distance = Mathf.Abs(currentLaneIndex - lane);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestLanes.Clear();
                    nearestLanes.Add(lane);
                }
                else if (distance == minDistance)
                {
                    nearestLanes.Add(lane);
                }
            }

            return nearestLanes[Random.Range(0, nearestLanes.Count)];
        }

        private void SwitchToLane(int newLaneIndex, bool forceSwitch = false)
        {
            // aiPath.preventMovingBackwards = false;
            StartCoroutine(SwitchLane(newLaneIndex, forceSwitch));

            // StartCoroutine(ReEnablePreventMovingBackwards());
        }

        public IEnumerator SwitchLane(int targetLaneIndex, bool forceSwitch = false)
        {
            if (isSwitchingLane || targetLaneIndex == currentLaneIndex)
                yield break;

            var targetSplineContainer = LaneManager.Instance[targetLaneIndex];
            SplineUtility.GetNearestPoint(targetSplineContainer.Spline, transform.position, out _, out var t);
            float totalLengthOfTargetSpline = targetSplineContainer.Spline.CalculateLength(targetSplineContainer.transform.localToWorldMatrix);
            ChangePointGraphMask(targetLaneIndex);
            aiPath.SearchPath();
            isSwitchingLane = true;
            if (!forceSwitch)
            {
                controller.detectCollisions = false;


                float offsetDistance = Mathf.Lerp(2, 4, currentSpeed / AgentSpeedRandomizer.Instance.maxSpeedLimit);

                float baseDistance = t * totalLengthOfTargetSpline;

                float forwardDistance = Mathf.Clamp(baseDistance + offsetDistance, 0, totalLengthOfTargetSpline);


                float laneToLaneSpace = 2f;
                float diagonalDistance = Mathf.Sqrt(laneToLaneSpace * laneToLaneSpace + forwardDistance * forwardDistance);


                float forwardT = forwardDistance / totalLengthOfTargetSpline;

                Vector3 targetPos = targetSplineContainer.Spline.EvaluatePosition(forwardT);


                float elapsed = 0f;
                float duration = offsetDistance / currentSpeed;
                // duration = Mathf.Clamp(duration, 0.5f, 2f);

                Vector3 startPos = transform.position;
                rvo.priority = switchingPriority;
                Path p = new ABPath();
                p.vectorPath = new List<Vector3> { startPos, targetPos };
                float progress = elapsed / duration;
                while (progress < 1) //elapsed < duration)
                {
                    progress = elapsed / duration;
                    Vector3 newPos = Vector3.Lerp(startPos, targetPos, progress);
                    transform.position = newPos;
                    aiPath.Teleport(newPos);


                    Vector3 direction = (targetPos - transform.position).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);

                    elapsed += Time.deltaTime;
                    yield return null;
                }
                aiPath.Teleport(targetPos);
                //transform.position = targetPos;
            }
            rvo.priority = basePriority;
            currentLaneIndex = targetLaneIndex;
            splineContainer = targetSplineContainer;
            totalLength2 = totalLengthOfTargetSpline;
            maskIndex = currentLaneIndex;
            controller.detectCollisions = true;
            ChangeDestination();
            yield return null;
            isSwitchingLane = false;
        }

        private IEnumerator ReEnablePreventMovingBackwards()
        {
            yield return new WaitForSeconds(0.5f);
            aiPath.preventMovingBackwards = true;
        }

        private bool IsLaneBlocked(int laneIndexToCheck)
        {
            var targetSpline = LaneManager.Instance[laneIndexToCheck].Spline;
            var totalLength = SplineUtility.CalculateLength(targetSpline, LaneManager.Instance[laneIndexToCheck].transform.localToWorldMatrix);

            //checking distance travelled in the target spline (LANE)
            float myT = SplineUtility.GetNearestPoint(targetSpline, transform.position, out _, out _);
            float myDistance = myT * totalLength;

            for (int index = 0; index < allControllers.Count; index++)
            {
                var otherAgent = allControllers[index];
                if (otherAgent == this || otherAgent == null) continue;

                if (otherAgent.currentLaneIndex != laneIndexToCheck) continue;

                float otherT = SplineUtility.GetNearestPoint(targetSpline, otherAgent.transform.position, out _, out _);
                float otherDistance = otherT * totalLength;

                // Check if other agent is ahead on the same spline and within detection range
                float distanceAhead = otherDistance - myDistance;
                if (distanceAhead > 0f && distanceAhead < lookAheadDetection)
                {
                    // someone is blocking ahead on this lane
                    return true;
                }
            }

            return false;
        }


        private void TryDynamicOvertake()
        {
            if (Time.time - lastLaneSwitchTime < laneSwitchCooldown) return;
            if (!isCurrentlyBlocked || currentSpeed < minSpeedToOvertake) return;

            float futureDistance = totalTravelledDistance + predictiveLookaheadDistance;
            float t = Mathf.InverseLerp(0, totalLength2, futureDistance);
            Vector3 predictedPos = SplineUtility.EvaluatePosition(splineContainer.Spline, t);

            foreach (int offset in new int[] { -1, 1 }) // Check left and right lanes
            {
                int newLane = currentLaneIndex + offset;
                if (!LaneManager.Instance.IsValidLaneIndex(newLane)) continue;

                var neighborSpline = LaneManager.Instance[newLane].Spline;
                float tOnNeighbor = SplineUtility.GetNearestPoint(neighborSpline, predictedPos, out _, out _);
                Vector3 testPos = SplineUtility.EvaluatePosition(neighborSpline, tOnNeighbor);

                bool canSwitch = true;

                foreach (var other in allAgents)
                {
                    if (other == aiPath || other == null) continue;

                    var otherCtrl = other.GetComponent<AgentController_RVO>();
                    if (otherCtrl == null || otherCtrl.currentLaneIndex != newLane) continue;

                    float otherDist = otherCtrl.totalTravelledDistance;
                    float myPriority = basePriority + speedPriorityWeight * currentSpeed;
                    float otherPriority = otherCtrl.basePriority + speedPriorityWeight * otherCtrl.currentSpeed;


                    if (Mathf.Abs(otherDist - futureDistance) < lookAheadDetection && myPriority <= otherPriority)
                    {
                        canSwitch = false;
                        break;
                    }
                }

                if (canSwitch)
                {
                    SwitchToLane(newLane);
                    lastLaneSwitchTime = Time.time;
                    break;
                }
            }
        }
    }
}
