using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AarquieSolutions.Base.Singleton;
using AarquieSolutions.InspectorAttributes;
using UnityEngine.Splines;

namespace EliminateRaceGame
{
    public class ObstacleSpawner : Singleton<ObstacleSpawner>
    {
        public List<ObstacleConfig> obstacles = new List<ObstacleConfig>();

        public List<SplineContainer> lanes => LaneManager.Instance.lanes;

        public static List<ObstacleConfig> ActiveObstacles => Instance.obstacles;


        private void Start()
        {
            foreach (var config in obstacles)
            {
                StartCoroutine(SpawnObstacle(config));
            }
        }

        private IEnumerator SpawnObstacle(ObstacleConfig config)
        {
            yield return new WaitForSeconds(config.delay);
            foreach (int laneIndex in config.affectedLanes)
            {
                if (LaneManager.Instance.IsValidLaneIndex(laneIndex))
                {
                    Vector3 position = LaneManager.Instance.GetPositionOnLane(laneIndex, config.tPosition);
                    GameObject obstacle = Instantiate(config.obstaclePrefab, position, Quaternion.identity);
                    var largeBoulder = obstacle.GetComponent<LargeBoulderTrigger>();
                    if (largeBoulder != null)
                    {
                        largeBoulder.affectedLanes = config.affectedLanes;
                        largeBoulder.spline = LaneManager.Instance[laneIndex];
                    }
                    var sidewaysBoulder = obstacle.GetComponent<SidewaysBoulderTrigger>();
                    if (sidewaysBoulder != null)
                    {
                        sidewaysBoulder.affectedLanes = config.affectedLanes;
                        sidewaysBoulder.startPos = position;
                    }
                    var yeti = obstacle.GetComponent<YetiSwipeTrigger>();
                    if (yeti != null)
                    {
                        yeti.affectedLanes = config.affectedLanes;
                    }
                    var tree = obstacle.GetComponent<FallingTreeTrigger>();
                    if (tree != null)
                    {
                        tree.affectedLanes = config.affectedLanes;
                    }
                    var plank = obstacle.GetComponent<BreakingPlankTrigger>();
                    if (plank != null)
                    {
                        plank.affectedLanes = new List<int>() { laneIndex };
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Existing gizmos
            // Add obstacle visualization
            if (ActiveObstacles != null)
            {
                foreach (var obstacle in ActiveObstacles)
                {
                    foreach (int lane in obstacle.affectedLanes)
                    {
                        Vector3 pos = LaneManager.Instance.GetPositionOnLane(lane, obstacle.tPosition);
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(pos, 0.5f);
                    }
                }
            }
        }

        // public void BroadCastObstacleTriggered(IObstacleProperties properties)
        // {
        //     currentObstacles.Add(properties);
        // }
        //
        // public void ReleaseObstacleTrigger(IObstacleProperties properties)
        // {
        //     if (currentObstacles.Contains(properties))
        //     {
        //         currentObstacles.Remove(properties);
        //     }
        // }
        //


       
    }
}
