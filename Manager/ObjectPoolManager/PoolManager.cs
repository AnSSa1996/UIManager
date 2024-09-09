using System;
using System.Collections.Generic;
using MonsterLove.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    public bool logStatus;
    public Dictionary<GameObject, Transform> RootTransform { get; private set; }
    private Dictionary<GameObject, ObjectPool<GameObject>> _prefabLookup;
    private Dictionary<GameObject, ObjectPool<GameObject>> _instanceLookup;

    private bool _dirty = false;

    protected override void Awake()
    {
        base.Awake();
        _prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
        RootTransform = new Dictionary<GameObject, Transform>();
    }

    private void Update()
    {
        if (logStatus && _dirty)
        {
            PrintStatus();
            _dirty = false;
        }
    }

    public void warmPool(GameObject prefab, int size)
    {
        if (_prefabLookup.ContainsKey(prefab))
        {
            throw new Exception("Pool for prefab " + prefab.name + " has already been created");
        }

        var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab); }, size);
        _prefabLookup[prefab] = pool;
        _dirty = true;
    }

    public GameObject spawnObject(GameObject prefab)
    {
        return spawnObject(prefab, Vector3.zero, Quaternion.identity);
    }

    public GameObject spawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (_prefabLookup.ContainsKey(prefab) == false)
        {
            WarmPool(prefab, 1);
        }

        var pool = _prefabLookup[prefab];

        var clone = pool.GetItem();
        clone.transform.SetPositionAndRotation(position, rotation);
        clone.SetActive(true);

        _instanceLookup.Add(clone, pool);
        _dirty = true;
        return clone;
    }

    public void releaseObject(GameObject clone)
    {
        clone.SetActive(false);

        if (RootTransform.TryGetValue(clone, out Transform value)) clone.transform.SetParent(value);
        if (_instanceLookup.ContainsKey(clone))
        {
            _instanceLookup[clone].ReleaseItem(clone);
            _instanceLookup.Remove(clone);
            _dirty = true;
        }
        else
        {
            Destroy(clone);
        }
    }


    private GameObject InstantiatePrefab(GameObject prefab)
    {
        var go = Instantiate(prefab);
        if (RootTransform.ContainsKey(go) == false)
        {
            var rootGameObject = new GameObject($"$RootTransform_{go.name}");
            var rootTransform = rootGameObject.transform;
            rootTransform.SetParent(transform);
            RootTransform.Add(go, rootTransform);
        }

        if (RootTransform.TryGetValue(go, out Transform value)) go.transform.SetParent(value);
        return go;
    }

    private void PrintStatus()
    {
        foreach (KeyValuePair<GameObject, ObjectPool<GameObject>> keyVal in _prefabLookup)
        {
            Debug.Log(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name,
                keyVal.Value.CountUsedItems, keyVal.Value.Count));
        }
    }

    private static void WarmPool(GameObject prefab, int size)
    {
        Instance.warmPool(prefab, size);
    }

    public static GameObject SpawnObject(GameObject prefab)
    {
        return Instance.spawnObject(prefab);
    }

    public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instance.spawnObject(prefab, position, rotation);
    }

    public static void ReleaseObject(GameObject clone)
    {
        Instance.releaseObject(clone);
    }
}