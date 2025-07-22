using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody),typeof(Collider))]
public class OnTriggerEvent : MonoBehaviour
{
    public event Action<Collider> OnTrigger;

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
        OnTrigger?.Invoke(other);
    }
}
