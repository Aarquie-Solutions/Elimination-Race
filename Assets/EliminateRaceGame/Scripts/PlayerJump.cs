using System.Collections;
using UnityEngine;

namespace ZombieElimination
{
    public partial class Player
    {
        public void JumpTo(Vector3 destination, float jumpHeight, float jumpDuration)
        {
            StartCoroutine(JumpArcCoroutine(transform.position, destination, jumpHeight, jumpDuration));
        }

        private IEnumerator JumpArcCoroutine(Vector3 start, Vector3 end, float height, float duration)
        {
            if (follower != null)
                follower.enabled = false;

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
        }
    }
}
