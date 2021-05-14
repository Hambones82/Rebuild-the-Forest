using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorUnitManager : MonoBehaviour
{
    private static ActorUnitManager _instance;
    public static ActorUnitManager Instance { get => _instance; }

    [SerializeField]
#pragma warning disable CS0649 // Field 'ActorUnitManager.gridMap' is never assigned to, and will always have its default value null
    private GridMap gridMap;
#pragma warning restore CS0649 // Field 'ActorUnitManager.gridMap' is never assigned to, and will always have its default value null

    [SerializeField]
#pragma warning disable CS0649 // Field 'ActorUnitManager.actorUnitPrefab' is never assigned to, and will always have its default value null
    private GameObject actorUnitPrefab;
#pragma warning restore CS0649 // Field 'ActorUnitManager.actorUnitPrefab' is never assigned to, and will always have its default value null

    private GameObjectPool actorUnitPool;
    //make an object pool for actor unit.  i guess it woudl have to be a prefab.

    private void Awake()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("cannot instantiate more than one BuildingManager");
        }
        _instance = this;
        actorUnitPool = new GameObjectPool(actorUnitPrefab, parentObj: gridMap.gameObject);
    }

    public void KillActorUnit(ActorUnit actorUnit)
    {
        actorUnit.gameObject.SetActive(false);
        actorUnitPool.RecycleObject(actorUnit.gameObject);
    }
}
