using System;
using System.Collections.Generic;
using System.Linq;
using AarquieSolutions.Base.Singleton;
using UnityEngine;
using ZombieElimination;

namespace ZombieElimination
{
    public class PlayersManager : Singleton<PlayersManager>
    {
        public List<Player> players;
        private static float updateInterval = 0.3f;
        private float updateTimer = 0f;

        private void Awake()
        {
            players = GetComponentsInChildren<Player>().ToList();
        }

        private void LateUpdate()
        {
            updateTimer += Time.deltaTime;
            if (updateTimer < updateInterval) return;
            updateTimer = 0f;
            players = players.OrderByDescending(x => x.progressTracker.totalProgress).ToList();
        }

        public Player GetPlayerWithLowestProgress()
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                var player = players[i];
                if (player.isEliminating)
                {
                    continue;
                }
                return player;
            }
            return players[^1];
        }
    }
}
