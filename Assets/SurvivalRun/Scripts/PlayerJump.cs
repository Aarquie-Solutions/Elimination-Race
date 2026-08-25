using System;
using System.Collections;
using UnityEngine;

namespace ZombieElimination
{
    public partial class Player : IJump
    {
        public void Jump(Vector3 destination, float jumpHeight, float jumpDuration, Action onComplete = null)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            StartCoroutine(JumpArcCoroutine(transform.position, destination, jumpHeight, jumpDuration, onComplete));
        }

        private IEnumerator JumpArcCoroutine(Vector3 start, Vector3 end, float height, float duration, Action onComplete = null)
        {
            if (follower != null)
                follower.enabled = false;

            //overriding duration
            float distance = start.DistanceXZ(end);
            duration = distance / speedHandler.CurrentSpeed;
            float elapsed = 0;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                Vector3 pos = Vector3.Lerp(start, end, t);
                pos.y += height * 4 * (t - t * t);

                transform.position = pos;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;

            // landing animation or fx

            if (follower != null)
                follower.enabled = true;

            onComplete?.Invoke();
        }
    }
}
