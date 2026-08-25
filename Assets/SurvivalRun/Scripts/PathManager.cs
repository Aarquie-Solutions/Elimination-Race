using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AarquieSolutions.Base.Singleton;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZombieElimination
{
    public class PathManager : MonoBehaviour
    {
        //assign this in aidestinationsetter for all players
        public Transform followTarget;
        public float updateRate = 1f;
        public float minDistance = 50f;
        // public List<Transform> routes = new List<Transform>();

        private void Start()
        {
            StartCoroutine(RePathFollowTarget());
            ServiceLocator.pathManager = this;
        }

        private IEnumerator RePathFollowTarget()
        {
            var wait = new WaitForSeconds(updateRate);
            while (true)
            {
                yield return wait;
                if (followTarget != null)
                {
                    foreach (Player player in ServiceLocator.playersManager.Players)
                    {
                        if (Vector3.Distance(player.transform.position, followTarget.position) < minDistance)
                        {
                            followTarget.position += Vector3.forward * minDistance;
                            break;
                        }
                    }
                }
                AstarPath.active.Scan();
            }
        }

        // public List<Transform> GetRoutePoints()
        // {
        //     var points = routes[0].GetComponentsInChildren<Transform>().ToList();
        //     points.RemoveAt(0);
        //     return points;
        // }



        // public static Vector3 GetRandomNavmeshPointNear(Transform referenceTransform, float radius = 5f, int maxAttempts = 20)
        // {
        //     var graph = AstarPath.active.data.recastGraph;
        //     NNConstraint constraint = NNConstraint.Walkable;
        //     constraint.graphMask = GraphMask.FromGraph(graph);
        //
        //     for (int i = 0; i < maxAttempts; i++)
        //     {
        //         Vector3 randomOffset = Random.insideUnitSphere * radius;
        //         randomOffset.y = 0; // Keep on ground plane
        //         Vector3 samplePos = referenceTransform.position + randomOffset;
        //
        //         var nnInfo = AstarPath.active.GetNearest(samplePos, constraint);
        //         if (nnInfo.node != null && nnInfo.node.Walkable)
        //         {
        //             return nnInfo.position;
        //         }
        //     }
        //
        //     Debug.LogWarning("Failed to find valid point on NavMesh");
        //     return referenceTransform.position;
        // }
    }
}
