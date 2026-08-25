using UnityEngine;
using UnityEngine.Pool;

public class AdvancedObjectPool<T> where T : Component
{
    private readonly ObjectPool<T> pool;
    private readonly T prefab; 
    private readonly Transform parent;

    public AdvancedObjectPool(
        T prefab,
        int defaultCapacity = 10,
        int maxSize = 100,
        Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        pool = new ObjectPool<T>(
            CreatePooledObject,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize);
    }

    private T CreatePooledObject()
    {
        var instance = Object.Instantiate(prefab, parent);
        instance.gameObject.SetActive(false);
        return instance;
    }

    private void OnTakeFromPool(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(T obj)
    {
        Object.Destroy(obj.gameObject);
    }

    public T Get()
    {
        return pool.Get();
    }

    public void Release(T obj)
    {
        pool.Release(obj);
    }

    public int CountInactive => pool.CountInactive;

    public int CountActive => pool.CountActive;
}
