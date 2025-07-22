using System.Collections;
using UnityEngine;

public class RagdollTest : MonoBehaviour
{
    [Header("Settings")]
    public float forceAmount = 500f;

    [Header("References")]
    public Camera cam;
    [SerializeField] private Transform animatedRoot;
    [SerializeField] private Transform ragdollRoot;

    private Animator animator;
    private Rigidbody[] ragdollBodies;
    public bool kinematic = false;

    private void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();

        DisableRagdoll();
    }

    public LayerMask ragdollLayerMask;
    public Transform anchor;

    private void Update()
    {
        // foreach (var rb in ragdollBodies)
        // {
        //     rb.isKinematic = kinematic;
        // }
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit1, Mathf.Infinity, ragdollLayerMask))
        {
            anchor.position = hit1.point;
            //Debug.Log($"{hit1.collider.name} hit");
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ragdollLayerMask))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

                if (rb != null && rb.isKinematic)
                {
                    StartCoroutine(ApplyForce(ray, rb, hit));
                }
            }
        }
    }

    private IEnumerator ApplyForce(Ray ray, Rigidbody rb, RaycastHit hit)
    {
        EnableRagdoll();
        yield return null; // Wait a frame for physics activation

        // Vector3 forceDir = ray.direction.normalized;
        var forceDir = Vector3.up;
        rb.AddForceAtPosition(forceDir * forceAmount, hit.point, ForceMode.Impulse);
        $"Adding force to {rb.name}".Log();
        Debug.DrawRay(hit.point, forceDir * 2f, Color.red, 2f);

        yield return new WaitForSeconds(5f); // Wait before re-enabling animator
        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        animator.enabled = false;

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

        animator.enabled = true;
    }
}
