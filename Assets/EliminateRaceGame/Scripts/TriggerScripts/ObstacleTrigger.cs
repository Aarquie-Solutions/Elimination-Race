using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EliminateRaceGame
{
    public abstract class ObstacleTrigger : MonoBehaviour, IObstacleProperties
    {
        [SerializeField] protected OnTriggerEvent onTriggerStart;
        [SerializeField] protected OnTriggerEvent onTriggerEnd;
        public List<int> affectedLanes;
        protected List<int> safeLanes;
        public EliminateTriggerProperties eliminateProperties;

        protected virtual void Start()
        {
            onTriggerStart.OnTrigger += OnTriggerStartActions;
            onTriggerEnd.OnTrigger += OnTriggerEndActions;
            Initialise();
        }

        protected abstract void Initialise();

        protected virtual void OnTriggerStartActions(Collider other)
        {
            if (other.TryGetComponent(out AgentController_RVO agent))
            {
                OnAgentTriggered(agent);
            }
        }

        protected virtual void OnTriggerEndActions(Collider other)
        {
            if (other.TryGetComponent(out AgentController_RVO agent))
            {
                agent.HandleForceSwitchLanes(null);
                // if (AgentController_RVO.allControllers.Find(x => x.eliminationTag == EliminationTag) == null)
                // {
                // }
            }
        }

        protected void SetEliminateProperties()
        {
            eliminateProperties = new EliminateTriggerProperties();
            eliminateProperties.properties = this as IObstacleProperties;
            eliminateProperties.agentsToEliminate ??= new();
            eliminateProperties.agentsToEliminate.Clear();
            foreach (AgentController_RVO rvo in agentsToEliminate)
            {
                eliminateProperties.agentsToEliminate.Add(rvo.AgentID);
            }
        }

        protected abstract void OnAgentTriggered(AgentController_RVO agent);

        protected List<AgentController_RVO> agentsToEliminate;

        public void AssignAgentsToEliminate()
        {
            int maxAgentsToEliminate = MaxAgentsToEliminate;
            if (maxAgentsToEliminate == 0)
            {
                $"IObstacleProperties.MaxAgentsToEliminate is 0 | {EliminationTag}".LogError();
            }
            agentsToEliminate ??= new List<AgentController_RVO>();
            agentsToEliminate.Clear();

            foreach (AgentController_RVO agent in AgentController_RVO.allControllers)
            {
                if (maxAgentsToEliminate > 0 && !agent.isWinner && agent.eliminationTag is EliminationTag.MoveToSafe or EliminationTag.None)
                {
                    agentsToEliminate.Add(agent);
                    maxAgentsToEliminate--;
                }
            }
            $"Agents to eliminate for {EliminationTag} set to {string.Join(",", agentsToEliminate.Select(x => x.gameObject.name))}".Log();
        }

        protected abstract bool CanActivateCondition();
        protected abstract void ActivateObstacle();


        protected void SetSafeLanes()
        {
            safeLanes ??= new();
            SafeLanes.Clear();
            for (int i = 0; i < LaneManager.Instance.LaneCount; i++)
            {
                if (!AffectedLanes.Contains(i))
                {
                    SafeLanes.Add(i);
                }
            }
            $"Safee lanes for {EliminationTag} set to {string.Join(",", SafeLanes)}".Log();
        }

        public bool IsSet { get; protected set; }

        public List<int> AffectedLanes
        {
            get => affectedLanes;
        }

        public List<int> SafeLanes
        {
            get => safeLanes;
        }

        public EliminationTag EliminationTag { get; protected set; }

        public int MaxAgentsToEliminate { get; protected set; }
    }
}
