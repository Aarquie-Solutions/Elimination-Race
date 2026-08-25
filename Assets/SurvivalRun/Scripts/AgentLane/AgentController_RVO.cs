using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;
using UnityEngine.Splines;

namespace EliminateRaceGame
{
    [RequireComponent(typeof(RVOController))]
    [RequireComponent(typeof(AIPath))]
    public partial class AgentController_RVO : MonoBehaviour
    {
        private static List<AIPath> allAgents = new List<AIPath>();
        public static List<AgentController_RVO> allControllers = new();
        private static int counterForName;
        public EliminateTriggerProperties currentEliminationProperties;

        [field: SerializeField] public int AgentID { get; private set; }

        public static int WinnerAgentID = -1;

        public static Transform parent;

        [Header("Detection Settings")]
        public float lookAheadDetection = 3f;
        public float checkForBlockRate;

        [Header("Speed Control")]
        public float speedAdjustRate = 3f;

        public bool isWinner = false;
        [Tooltip("Obstacle type that eliminates this agent")]
        public EliminationTag eliminationTag = EliminationTag.None;

        public float currentSpeed;
        public float originalMaxSpeed;
        public float targetSpeed;
        public float maxSpeedToReach;
        public bool isCurrentlyBlocked;
        public GraphMask graphMask;

        [Header("Spline")]
        public SplineContainer splineContainer;
        public float totalLength2;
        public float totalTravelledDistance;
        public float updateTargetDistance = 20f;
        public float changeDistanceAt = 5f;
        public float targetSetDistanceAt;

        //
        public int maskIndex;

        public bool WillDie => eliminationTag != EliminationTag.MoveToSafe && eliminationTag != EliminationTag.None;

        private AIPath aiPath;
        private AIDestinationSetter destinationSetter;
        private Transform destinationTarget;

        private bool isInitialized;

        private RVOController rvo;
        private Seeker seeker;

        void Awake()
        {
            rvo = GetComponent<RVOController>();
            aiPath = GetComponent<AIPath>();
            seeker = GetComponent<Seeker>();
            destinationSetter = GetComponent<AIDestinationSetter>();
            controller = GetComponent<CharacterController>();

            splineContainer = null;
        }

        private void Start()
        {
            ValidateNameAndDestination();

            if (splineContainer == null)
            {
                splineContainer = LaneManager.Instance.GetNearestLane(transform.position, out int laneIndexToFollow);
                SwitchToLane(laneIndexToFollow, true);
            }
            else // For Testing hardcoded gameobject name
            {
                if (gameObject.name.Contains("0"))
                {
                    ChangePointGraphMask(0);
                }
                else
                {
                    ChangePointGraphMask(1);
                }
            }

            InitialiseAgent();
            isInitialized = true;
        }


        void FixedUpdate()
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, speedAdjustRate * Time.deltaTime);

            aiPath.maxSpeed = currentSpeed;

            if (forceSwitchLaneIndex == -1)
            {
                if (isCurrentlyBlocked && !isJumping)
                {
                    TrySwitchLanes();
                }
            }
            else
            {
                if (!isSwitchingLane)
                {
                    MoveCloserToForceTargetlane();
                }
            }
            Vector3 velocity = aiPath.velocity; // Or aiPath.velocity
            Vector3 forward = transform.forward;

            float directionDot = Vector3.Dot(forward.normalized, velocity.normalized);

            if (directionDot > 0.1f)
            {
                //  Debug.Log($" player {gameObject.name} Moving Forward");
            }
            else if (directionDot < -0.1f)
            {
                Debug.Log($" player {gameObject.name} {aiPath.desiredVelocity}  Moving Backward");
            }
            Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + aiPath.desiredVelocity, Color.red);
            UpdateTravelDistanceAndRotate();
            UpdateDestination();
            // for testing in runtime
            if (currentLaneIndex != maskIndex && isInitialized)
            {
                SwitchToLane(maskIndex);
            }
        }

        void OnEnable()
        {
            allAgents.Add(aiPath);
            allControllers.Add(this);
        }

        void OnDisable()
        {
            allAgents.Remove(aiPath);
            allControllers.Remove(this);
        }

        private void LateUpdate()
        {
            UpdateDebugText();
        }

        private void OnDrawGizmos()
        {
            if (transform == null)
            {
                return;
            }
            var endPos = transform.position + transform.forward * lookAheadDetection;
            Gizmos.color = isCurrentlyBlocked ? Color.red : Color.green;
            Gizmos.DrawSphere(endPos, 0.3f);
            Gizmos.DrawLine(transform.position, endPos);

            if (!destinationTarget)
            {
                return;
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, destinationTarget.position);
            Gizmos.DrawSphere(destinationTarget.position, 0.3f);
        }


        private void OnValidate()
        {
            graphMask = 1 << maskIndex;
        }

        private void InitialiseAgent()
        {
            destinationSetter.target = destinationTarget;
            originalMaxSpeed = aiPath.maxSpeed;
            currentSpeed = originalMaxSpeed;
            totalLength2 = SplineUtility.CalculateLength(splineContainer.Spline, splineContainer.transform.localToWorldMatrix);
            totalTravelledDistance = 0;

            aiPath.preventMovingBackwards = true;
            StartCoroutine(ICheckFrontAgentsAndAdjustSpeed());
        }

        private void ValidateNameAndDestination()
        {
            parent ??= LaneAgentSpawner.Instance.transform;

            transform.parent = parent;
            gameObject.name = "Agent Racer_" + ++counterForName;
            AgentID = counterForName;

            if (!destinationTarget)
            {
                destinationTarget = new GameObject("Destination Target " + gameObject.name).transform;
            }
            else
            {
                destinationTarget.name = "Destination Target " + gameObject.name;
            }
            if (counterForName == 1)
            {
                isWinner = true;
                WinnerAgentID = AgentID;
            }
        }

        private float lastTForTravelDistance;

        private void UpdateTravelDistanceAndRotate()
        {
            if (splineContainer == null)
            {
                return;
            }
            SplineUtility.GetNearestPoint(splineContainer.Spline, transform.position, out var pos1, out var t);
            t = Mathf.Max(t, lastTForTravelDistance);
            lastTForTravelDistance = t;

            totalTravelledDistance = t * totalLength2;
            Vector3 forward = SplineUtility.EvaluateTangent(splineContainer.Spline, t);
            if (forward != Vector3.zero)
            {
                transform.forward = forward;
            }
        }

        void UpdateDestination()
        {
            if (splineContainer == null)
            {
                return;
            }
            if (targetSetDistanceAt - totalTravelledDistance < changeDistanceAt)
            {
                ChangeDestination();
            }
        }

        void ChangeDestination()
        {
            targetSetDistanceAt = Mathf.Max(targetSetDistanceAt, totalTravelledDistance + updateTargetDistance);
            float t = Mathf.InverseLerp(0, totalLength2, targetSetDistanceAt);
            if (t > lastTForTravelDistance)
            {
                // $"player {gameObject.name}  chaning from {lastTForTravelDistance} to {t}".Log();
                Vector3 destination = splineContainer.Spline.EvaluatePosition(t);
                destinationTarget.position = destination;
            }
        }

        private IEnumerator ICheckFrontAgentsAndAdjustSpeed()
        {
            WaitForSeconds waitforChecking = new WaitForSeconds(checkForBlockRate);
            while (true)
            {
                isCurrentlyBlocked = false;
                if (splineContainer == null)
                {
                    continue;
                }
                targetSpeed = maxSpeedToReach;


                for (int index = 0; index < allControllers.Count; index++)
                {
                    var otherAgent = allControllers[index];
                    if (otherAgent == this || otherAgent == null) continue;
                    if (otherAgent.currentLaneIndex == this.currentLaneIndex)
                    {
                        float gap = otherAgent.totalTravelledDistance - this.totalTravelledDistance;
                        if (gap > 0 && gap < lookAheadDetection)
                        {
                            float otherSpeed = otherAgent.currentSpeed;
                            if (otherSpeed < targetSpeed)
                            {
                                targetSpeed = otherSpeed;
                            }
                        }
                    }
                }
                isCurrentlyBlocked = targetSpeed != maxSpeedToReach;
                yield return waitforChecking;
            }
        }

        public void SetMaxSpeedToReach(float newSpeed)
        {
            maxSpeedToReach = newSpeed;
        }

        public void Eliminate()
        {
            if (isWinner) return;
            aiPath.enabled = false;
            rvo.enabled = false;
            seeker.enabled = false;
            controller.enabled = false;
            if (!TryGetComponent<Rigidbody>(out var rb))
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            if (controller != null)
                controller.enabled = false;

            allAgents.Remove(aiPath);
            allControllers.Remove(this);

            LaneManager.Instance.OnAgentEliminated(this);

            StartCoroutine(DestroyAfterTime(2f));
        }

        IEnumerator DestroyAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }

        private void ChangePointGraphMask(int index)
        {
            maskIndex = index;
            graphMask = 1 << index;
            seeker.graphMask = graphMask;
        }
    }
}
