using System.Collections.Generic;
using System.Linq;
using AarquieSolutions.Base.Singleton;
using Pathfinding;
using UnityEngine;
using UnityEngine.Splines;

namespace EliminateRaceGame
{
    public class LaneManager : Singleton<LaneManager>
    {
        public AstarPath astarPath;
        public List<SplineContainer> lanes;

        public int graphs;
        public int graphIndex;

        public int LaneCount => lanes.Count;

        public SplineContainer this[int index] => (index >= 0 && index < lanes.Count) ? lanes[index] : null;

        public override void Awake()
        {
            base.Awake();

            Time.timeScale = 1;
        }

        private void OnValidate()
        {
            astarPath ??= AstarPath.active;
            var graphs = astarPath.graphs;

            bool shouldScan = false;
            for (int i = 0; i < graphs.Length; i++)
            {
                var pointGraph = graphs[i] as PointGraph;
                if (pointGraph == null) continue;

                if (pointGraph.root == null)
                {
                    shouldScan = true;
                }
                pointGraph.root = lanes[i].transform;
            }
            if (shouldScan)
            {
                astarPath.Scan();
            }
        }

        public bool IsValidLaneIndex(int index) => index >= 0 && index < lanes.Count;

        public int GetLaneIndex(SplineContainer spline) => lanes.IndexOf(spline);
        public int GetLaneIndex(Vector3 position) => GetLaneIndex(GetNearestLane(position));

        public SplineContainer GetNearestLane(Vector3 position)
        {
            //lanes.OrderBy(x=> Vector3.Distance(x.transform.position, position));
            return lanes.OrderBy(x => Vector3.Distance(x.EvaluatePosition(0), position)).First();
        }

        public Vector3 GetPositionOnLane(int laneIndex, float t)
        {
            var spline = this[laneIndex];
            if (spline != null && spline.Evaluate(t, out var pos, out _, out _))
            {
                return pos;
            }
            return Vector3.zero;
            // return GetLaneSpline(laneIndex).EvaluatePosition(t);
        }

        public SplineContainer GetRandomLane(out int index)
        {
            index = Random.Range(0, lanes.Count);
            return lanes[index];
        }

        public SplineContainer GetNearestLane(Vector3 worldPosition, out int index)
        {
            index = 0;
            SplineContainer splineContainer = null;
            float minDistance = float.MaxValue;
            for (int i = 0; i < lanes.Count; i++)
            {
                var lane = lanes[i];
                var length = SplineUtility.CalculateLength(lane.Spline, lane.transform.localToWorldMatrix);
                SplineUtility.GetNearestPoint(lane.Spline, worldPosition, out var nearestPos, out var t);

                float distance = Vector3.Distance(worldPosition, nearestPos);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                    splineContainer = lane;
                }
            }
            return splineContainer;
        }

        public void ClearLanes()
        {
            lanes.Clear();
        }

        public void AddLane(SplineContainer lane)
        {
            lanes.Add(lane);
        }

        public void OnAgentEliminated(AgentController_RVO agentControllerRvo)
        {
        }
    }
}
