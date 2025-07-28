using System;
using UnityEngine;

namespace ZombieElimination
{
    public class ServiceLocator : MonoBehaviour
    {
        public static GameRulesSO gameRules;
        public static PlayersManager playersManager;
        public static PathManager pathManager;
        public static ZombieHordeController zombieHordeController;

        public static ServiceLocator Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                new EventManager();
                // gameRules = Resources.Load<GameRulesSO>("GameRules");
                // playersManager = GetComponent<PlayersManager>();
                // pathManager = GetComponent<PathManager>();
                // zombieHordeController = GetComponent<ZombieHordeController>();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
