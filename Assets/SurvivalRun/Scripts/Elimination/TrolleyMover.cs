using UnityEngine;

namespace ZombieElimination
{
    [RequireComponent(typeof(Rigidbody))]
    public class TrolleyMover : MonoBehaviour
    {
        private float arrivalThreshold = 0.05f;

        public Transform[] wheels;
        public float wheelRadius = 0.3f;

        private Rigidbody rb;
        private Vector3 targetPosition;
        private bool isMoving = false;
        private float totalTime;
        private float elapsedTime;
        private Vector3 startPosition;
        private Vector3 moveDirection;
        private float moveSpeed;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
        }

        private void FixedUpdate()
        {
            if (!isMoving) return;

            elapsedTime += Time.fixedDeltaTime;

            if (elapsedTime >= totalTime)
            {
                rb.MovePosition(targetPosition);
                StopMoving();
                return;
            }

            Vector3 linearVelocity = moveDirection * moveSpeed;
            linearVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = linearVelocity;
            RotateWheels();
        }

        private void RotateWheels()
        {
            if (rb == null || wheels == null || wheels.Length == 0)
                return;

            float angularVelocity = 0f;

            Vector3 horizontalVelocity = rb.linearVelocity;
            horizontalVelocity.y = 0f;
            float linearSpeed = horizontalVelocity.magnitude;

            if (wheelRadius > 0f)
                angularVelocity = linearSpeed / wheelRadius;

            float degreesPerFrame = Mathf.Rad2Deg * angularVelocity * Time.fixedDeltaTime;

            foreach (var wheel in wheels)
            {
                wheel.Rotate(degreesPerFrame, 0f, 0f, Space.Self);
            }
        }

        /// <summary>
        /// Start moving so we arrive at destination in exactly timeToReach seconds.
        /// </summary>
        public void MoveToInTime(Vector3 destination, float timeToReach)
        {
            if (timeToReach <= 0f)
            {
                Debug.LogWarning("Time to reach must be greater than zero.");
                return;
            }

            startPosition = transform.position;
            targetPosition = destination;
            totalTime = timeToReach;
            elapsedTime = 0f;

            Vector3 flatStart = new Vector3(startPosition.x, 0, startPosition.z);
            Vector3 flatEnd = new Vector3(destination.x, 0, destination.z);

            float distance = Vector3.Distance(flatStart, flatEnd);
            moveDirection = (destination - startPosition).normalized;
            moveDirection.y = 0f;

            transform.forward = moveDirection;
            
            moveSpeed = distance / timeToReach; // units/sec to match given time
            isMoving = true;
        }

        public void StopMoving()
        {
            isMoving = false;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
