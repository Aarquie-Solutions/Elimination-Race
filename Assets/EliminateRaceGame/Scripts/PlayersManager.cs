using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AarquieSolutions.Base.Singleton;
using Unity.Cinemachine;
using UnityEngine;
using ZombieElimination;
using System.Runtime.InteropServices;

namespace ZombieElimination
{
    public class PlayersManager : MonoBehaviour
    {
        [SerializeField] private List<Player> players;
        public int winnerIndex;

        public List<Player> Players
        {
            get
            {
                UpdatePlayerTransforms();
                return players;
            }
            set
            {
                players = value;
                UpdatePlayerTransforms();
            }
        }

        private void UpdatePlayerTransforms()
        {
            if (players.Count != playerTransforms?.Count)
            {
                playerTransforms = players.Select(x => x.transform).ToList();
            }
        }

        private List<Transform> playerTransforms;
        private static float updateInterval = 0.1f;
        private float updateTimer = 0f;
        public Transform grouperTrigger;

        private void Awake()
        {
            Players = GetComponentsInChildren<Player>().ToList();
            ServiceLocator.playersManager = this;
        }

        private void Start()
        {
            UpdateGrouper();
        }

        private void OnEnable()
        {
            EventManager.Instance.OnPlayerEliminated += OnPlayerEliminated;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnPlayerEliminated -= OnPlayerEliminated;
        }

        private void OnPlayerEliminated(Player obj)
        {
            Players.Remove(obj);
            UpdateGrouper();
            UpdatePlayerTransforms();
        }

        private void UpdateGrouper()
        {
            // grouperTrigger.Targets = new List<CinemachineTargetGroup.Target>();
            // for (int i = 0; i < playerTransforms.Count; i++)
            // {
            //     CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target();
            //     target.Object = playerTransforms[i].transform;
            //     target.Weight = 1f;
            //     grouperTrigger.Targets.Add(target);
            // }

            var centroid = TransformGrouper.CalculateCentroid(playerTransforms);
            centroid.y = 0;
            centroid.x = 0;
            grouperTrigger.transform.position = centroid;
            players = players.OrderByDescending(x => x.transform.position.z).ToList();
        }


        private void LateUpdate()
        {
            updateTimer += Time.deltaTime;
            if (updateTimer < updateInterval)
            {
                return;
            }
            updateTimer = 0f;
            UpdateGrouper();
        }

        public Player GetPlayerWithLowestProgress()
        {
            return players.LastOrDefault(p => !p.isEliminating && !p.isWinner);
        }

        public static class TransformGrouper
        {
            public static Vector3 CalculateCentroid(List<Transform> transforms)
            {
                if (transforms == null || transforms.Count == 0) return Vector3.zero;

                Vector3 sum = Vector3.zero;
                foreach (var t in transforms)
                {
                    sum += t.position;
                }
                return sum / transforms.Count;
            }
        }
    }
}
