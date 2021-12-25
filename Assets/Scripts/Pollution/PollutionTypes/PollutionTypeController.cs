using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PollutionTypeController //maybe make this generic for a pollution type???  not 100% sure...need to think abt it.
{
    [SerializeField]
    protected PollutionManager pollutionManager;

    [SerializeField]
    protected GridMap gridMap;
    [SerializeField]
    protected GridSubMap pollutionMap;
    [SerializeField]
    protected Pollution pollutionPrefab;
    [SerializeField]
    protected List<Pollution> pollutionObjects = new List<Pollution>();

    protected GameObjectPool pollutionPool;
    
    [SerializeField]
    private float spreadPeriod;
    private float spreadTimer = 0;

    public void UpdateState()
    {
        spreadTimer += Time.deltaTime;
        if (spreadTimer >= spreadPeriod)
        {
            spreadTimer = 0;
            UpdatePollutionState();
        }
    }

    protected abstract void UpdatePollutionState();

    public virtual void Initialize(GridMap inGridMap, PollutionManager inPollutionManager)
    {
        gridMap = inGridMap;
        pollutionManager = inPollutionManager;
        pollutionPool = new GameObjectPool(pollutionPrefab.gameObject, parentObj: gridMap.gameObject, activeByDefault: false);
    }
    
    public virtual void RemovePollution(Pollution pollution, Vector2Int pollutionPosition)
    {
        pollutionObjects.Remove(pollution);
        pollutionPool.RecycleObject(pollution.gameObject);
    }
}
