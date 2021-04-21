using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameObjectPool
{
    protected Stack<GameObject> availableObjects = new Stack<GameObject>();
    protected static readonly int initialNumber = 10;

    public IPoolObjectCreator poolObjectCreator;

    GameObject parent;

    bool active = true;

    public GameObjectPool(GameObject prefab = null, IPoolObjectCreator objectCreator = null, GameObject parentObj = null, bool activeByDefault = true)
    {
        active = activeByDefault;
        if (parentObj != null)
        {
            //Debug.Log("setting pool parent");
            parent = parentObj;
        }
        if (objectCreator == null)
        {
            if (prefab == null) throw new ArgumentException("prefab or object creator must be specified");
            poolObjectCreator = new DefaultObjectPoolCreator(prefab);
        }
        else
        {
            poolObjectCreator = objectCreator;
        }
        for (int i = 0; i < initialNumber; i++)
        {
            availableObjects.Push(poolObjectCreator.CreateNewPoolObject(parent, active: active));
        }
    }

    public GameObject GetGameObject()
    {
        GameObject returnObject;
        if (availableObjects.Count > 0)
        {
            returnObject = availableObjects.Pop();
        }
        else
        {
            returnObject = poolObjectCreator.CreateNewPoolObject(parent);
        }
        returnObject.SetActive(active);
        return returnObject;
    }

    public void RecycleObject(GameObject go)
    {
        IPoolObjectRecycler recycler = go.GetComponent<IPoolObjectRecycler>();
        if (recycler != null)
            recycler.RecycleObject();
        go.SetActive(false);
        availableObjects.Push(go);
    }
}
