using UnityEngine;
using System.Collections;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine.Splines;

namespace EliminateRaceGame
{
    [RequireComponent(typeof(CharacterController))]
    public partial class AgentController_RVO
    {
        [Header("Jump Properties")]
        public float gapCheckDistance = 2f;
        public float gapDetectionHeight = 2f;
        public float jumpHeight = 2f;
        public float jumpDuration = 1f;

        private CharacterController controller;
        private bool isJumping = false;



        public void StartJump(Vector3 jumpStart, Vector3 jumpEnd, float height, float duration)
        {
            if (!isJumping)
            {
                StartCoroutine(JumpRoutine(jumpStart, jumpEnd, height, duration));
            }
        }

        private IEnumerator JumpRoutine(Vector3 start, Vector3 end, float jumpHeight, float jumpDuration)
        {
            isJumping = true;

            // GameObject jumpPoint = new GameObject("JumpPoint " + gameObject.name);
            // GameObject landingPoint = new GameObject("LandingPoint" + gameObject.name);
            SplineUtility.GetNearestPoint(splineContainer.Spline, start, out var nearest, out _);


            SplineUtility.GetNearestPoint(splineContainer.Spline, end, out var nearest1, out var t1);
            //
            // if (willDie)
            // {
            //     var dist = t1 * totalLength2;
            //     dist -= 2;
            //     nearest1 = SplineUtility.EvaluatePosition(splineContainer.Spline, dist / totalLength2);
            // }

            start = nearest;
            end = nearest1;
            start.y = transform.position.y;
            end.y = start.y;

            // jumpPoint.transform.position = start;
            // landingPoint.transform.position = end;

            aiPath.destination = start;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, start) < 0.2f);

            aiPath.enabled = false;
            aiPath.canMove = false;
            rvo.enabled = false;

            float elapsed = 0f;
            // $"{gameObject.name} Jump Started;".Log();

            while (elapsed < jumpDuration)
            {
                float t = elapsed / jumpDuration;
                float heightOffset = 4 * jumpHeight * t * (1 - t); // parabola peak at t=0.5
                Vector3 horizontalPos = Vector3.Lerp(start, end, t);
                Vector3 jumpPos = horizontalPos + Vector3.up * heightOffset;

                controller.Move(jumpPos - transform.position);

                elapsed += Time.deltaTime;
                if (WillDie && !isWinner)
                {
                    if (t >= 0.7f)
                    {
                        break;
                    }
                }
                yield return null;
            }

            if (!WillDie)
            {
                controller.Move(end - transform.position);
            }

            isJumping = false;
            aiPath.enabled = true;
            aiPath.canMove = true;
            rvo.enabled = true;
            if (WillDie)
            {
                Eliminate();
            }
            // $"{gameObject.name} Jump Done;".Log();
            // Destroy(jumpPoint);
            // Destroy(landingPoint);
        }
    }
}
