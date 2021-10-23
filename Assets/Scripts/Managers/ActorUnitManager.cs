using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorUnitManager : MonoBehaviour
{
    private static ActorUnitManager _instance;
    public static ActorUnitManager Instance { get => _instance; }

    [SerializeField]
    private GridMap gridMap;

    [SerializeField]
    private GameObject actorUnitPrefab;

    private GameObjectPool actorUnitPool;
    //make an object pool for actor unit.  i guess it woudl have to be a prefab.

    private void Awake()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("cannot instantiate more than one BuildingManager");
        }
        _instance = this;
        actorUnitPool = new GameObjectPool(actorUnitPrefab, parentObj: gridMap.gameObject, activeByDefault: false);
    }

    public void KillActorUnit(ActorUnit actorUnit)
    {
        actorUnit.gameObject.SetActive(false);
        actorUnitPool.RecycleObject(actorUnit.gameObject);
    }
}
