using UnityEngine;
using ZombieElimination;

public class Block : MonoBehaviour
{
    public int blockID;
    public int blockIndex;
    public OnTriggerEvent onTriggerEvent;

    private Transform endPoint, pivotPoint;

    public Transform EndPoint => endPoint;

    public Transform PivotPoint => pivotPoint;

    // Optional: reset state before reuse (override in subclasses if needed)
    public virtual void ResetBlockState()
    {
        // Reset visuals, clear obstacles, etc.
    }

    private void OnEnable()
    {
        if (onTriggerEvent != null)
            onTriggerEvent.OnTrigger += OnBlockTriggered;
        endPoint = transform.Find("EndPoint");
        pivotPoint = transform.Find("PivotPoint");
    }

    private void OnBlockTriggered(Collider obj)
    {
        if (!obj.gameObject.name.Equals(ServiceLocator.playersManager.grouperTrigger.name))
        {
            return;
        }
        EventManager.Instance.TriggerBlockTriggered(this);
    }

    private void OnDisable()
    {
        if (onTriggerEvent != null)
            onTriggerEvent.OnTrigger -= OnBlockTriggered;
    }
}
