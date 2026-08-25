using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(Collider))]
public class OnTriggerEvent : MonoBehaviour
{
    public event Action<Collider> OnTriggerEnterEvent;
    public event Action<Collider> OnTriggerExitEvent;

    private void OnValidate()
    {
        Awake();
    }

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    private void OnCollisionEnter(Collision other)
    {
        OnTriggerEnterEvent?.Invoke(other.collider);
    }
    private void OnCollisionExit(Collision other)
    {
        OnTriggerExitEvent?.Invoke(other.collider);
    }
}
