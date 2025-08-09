using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RagdollBody : MonoBehaviour
{
    [Header("Settings")]
    public float forceAmount = 500f;

    [Header("References")]
    [SerializeField] private Transform animatedRoot;
    [SerializeField] private Transform ragdollRoot;

    // private Animator animator;
    private Rigidbody[] ragdollBodies;
    public Rigidbody middleBody;
    public bool kinematic = false;

    private void Awake()
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
    }

    private void Start()
    {
        // animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>(true);
        DisableRagdoll();
        gameObject.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            var randomCollider = GetComponentsInChildren<Collider>()[Random.Range(0, ragdollBodies.Length)];
            Vector3 closePoint = randomCollider.ClosestPoint(randomCollider.transform.position + Vector3.up);
            // Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            // if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ragdollLayerMask))
            {
                Rigidbody rb = randomCollider.GetComponent<Rigidbody>();

                if (rb != null && rb.isKinematic)
                {
                    StartCoroutine(ApplyForce(rb, closePoint));
                }
            }
        }
    }

    public void ApplyForce(Vector3 point)
    {
        gameObject.SetActive(true);
        StartCoroutine(ApplyForce(middleBody, point));
        // var randomCollider = GetComponentsInChildren<Collider>()[Random.Range(0, ragdollBodies.Length)];
        // Vector3 closePoint = randomCollider.ClosestPoint(randomCollider.transform.position + Vector3.up);
        // StartCoroutine(ApplyForce(randomCollider.GetComponent<Rigidbody>(), closePoint));
    }

    private IEnumerator ApplyForce(Rigidbody rb, Vector3 point)
    {
        EnableRagdoll();
        yield return null; // Wait a frame for physics activation

        var forceDir = Vector3.up + Vector3.back;
        rb.AddForceAtPosition(forceDir * forceAmount, point, ForceMode.Impulse);
        $"Adding force to {rb.name}".Log();
        // Debug.DrawRay(point, forceDir * 2f, Color.red, 2f);

        // yield return new WaitForSeconds(5f); // Wait before re-enabling animator
        // DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        // animator.enabled = false;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = false;
        }
    }

    public void DisableRagdoll()
    {
        if (animatedRoot && ragdollRoot)
        {
            animatedRoot.position = ragdollRoot.position;
            animatedRoot.rotation = ragdollRoot.rotation;
        }

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
        }

        // animator.enabled = true;
    }
}
