using System.Collections.Generic;
using AarquieSolutions.InspectorAttributes;
using UnityEngine;

namespace EliminateRaceGame
{
    [System.Serializable]
    public struct ObstacleConfig
    {
        public GameObject obstaclePrefab;
        [ShowIf("IsJumpObstacle")]
        public List<int> affectedLanes;
        public float tPosition;
        public float delay;
        public EliminationTag eliminationTag; // New field for obstacle type

        public bool IsJumpObstacle() => eliminationTag != EliminationTag.Jump;
    }

    public interface IObstacleProperties
    {
        public bool IsSet { get; }

        public List<int> AffectedLanes { get; }

        public List<int> SafeLanes { get; }

        public EliminationTag EliminationTag { get; }

        public int MaxAgentsToEliminate { get; }
    }

    [System.Serializable]
    public class EliminateTriggerProperties
    {
        public IObstacleProperties properties;
        public List<int> agentsToEliminate;
    }

    // public struct ObstacleTriggerProperties
    // {
    //     public List<int> affectedLanes;
    //     public List<int> safeLanes;
    //     public EliminationTag eliminationTag;
    //     public float maxTravelDistance;
    //
    //     public ObstacleTriggerProperties(EliminationTag eliminationTag, float maxTravelDistance, IObstacleProperties properties)
    //     {
    //         this.affectedLanes = new List<int>();
    //         this.safeLanes = new List<int>();
    //         this.eliminationTag = eliminationTag;
    //         this.maxTravelDistance = maxTravelDistance;
    //
    //         affectedLanes.Clear();
    //         safeLanes.Clear();
    //         for (int i = 0; i < LaneManager.Instance.LaneCount; i++)
    //         {
    //             if (properties.AffectedLanes.Contains(i))
    //             {
    //                 affectedLanes.Add(i);
    //             }
    //             else
    //             {
    //                 safeLanes.Add(i);
    //             }
    //         }
    //     }
    // }
}
