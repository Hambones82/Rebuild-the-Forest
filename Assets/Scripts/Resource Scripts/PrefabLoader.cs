using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class PrefabLoader<T, P> where T : System.Enum where P : PrefabLoader<T, P>
{
    private static readonly Lazy<P> Lazy =
        new Lazy<P>(() => Activator.CreateInstance(typeof(P), true) as P);

    public static P Instance => Lazy.Value;

    protected abstract Dictionary<T, string> prefabPaths
    {
        get;
    }
    

    protected Dictionary<T, GameObject> loadedPrefabs = new Dictionary<T, GameObject>();

    public virtual void Initialize()
    {
        foreach(KeyValuePair<T, string> kvp in prefabPaths)
        {
            loadedPrefabs.Add(kvp.Key, Resources.Load(prefabPaths[kvp.Key]) as GameObject);
        }
    }

    public GameObject GetPrefab(T key)
    {
        return loadedPrefabs[key];
    }

    public GameObject InstantiatePrefab(T key, Transform parent)
    {
        return GameObject.Instantiate(loadedPrefabs[key], parent);
    }
        
}
