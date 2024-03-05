using System.Collections.Generic;
using UnityEngine;

public interface OnReturnPool<PoolType>
{
    void Init(OnReturnPoolEvent<PoolType> onReturnPoolEvent);
}

public delegate void OnReturnPoolEvent<PoolType>(PoolType type);

public class ObjectPooling<ManagerType, PoolType> : GenericSingleton<ManagerType>
    where PoolType : MonoBehaviour, OnReturnPool<PoolType>
    where ManagerType : MonoBehaviour
{
    private Transform storageParent;
    [SerializeField] PoolType prefab;
    [SerializeField] private int poolCount;

    [SerializeField] private Stack<PoolType> pool;

    protected new void Awake()
    {
        base.Awake();

        pool = new Stack<PoolType>();

        storageParent = new GameObject("StorageParent").transform;
        storageParent.SetParent(transform);
        storageParent.gameObject.SetActive(false);

        Pooling();
    }

    private void Pooling()
    {
        for (int i = 0; i < poolCount; i++)
        {
            CreateObject();
        }
    }

    private PoolType CreateObject()
    {
        PoolType newObj = Instantiate(prefab, storageParent);

        newObj.Init(OnReturnUp);

        pool.Push(newObj);

        return newObj;
    }

    public PoolType GetPool()
    {
        if (pool.Count <= 0)
        {
            return CreateObject();
        }

        PoolType obj = pool.Pop();
        obj.transform.SetParent(storageParent);

        return obj;
    }

    private void OnReturnUp(PoolType poolObject)
    {
        poolObject.transform.SetParent(storageParent);

        pool.Push(poolObject);
    }
}