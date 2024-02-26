using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PollutionTypeController //maybe make this generic for a pollution type???  not 100% sure...need to think abt it.
{
    public delegate void PollutionChangeDelegate(Vector2Int cell, int priority);
    public event PollutionChangeDelegate OnPollutionAdd;
    public event PollutionChangeDelegate OnPollutionDelete;
    protected void InvokeOnPollutionAdd(Vector2Int cell, int priority) { OnPollutionAdd?.Invoke(cell, priority); }
    protected void InvokeOnPollutionDelete(Vector2Int cell, int priority) { OnPollutionDelete?.Invoke(cell, priority); }

    [SerializeField]
    protected PollutionManager pollutionManager;

    [SerializeField]
    protected List<Vector2Int> freePositions;
    public List<Vector2Int> FreePositions { get => freePositions; }

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

    public abstract void InitializePollutionState();

    public virtual void RemovePollution(Pollution pollution, Vector2Int pollutionPosition)
    {
        if(pollution.pTypeController == this)//get rid of this -- it's definitely hurting performance.
        {
            pollutionObjects.Remove(pollution);
            pollutionPool.RecycleObject(pollution.gameObject);
            OnPollutionDelete?.Invoke(pollutionPosition, pollution.Priority);
        }
    }

    //priorities are the priority of the 
    public abstract void UpdateFreePositionsForAddition(Vector2Int cell, int priority);
    public abstract void UpdateFreePositionsForRemoval(Vector2Int cell, int priority);

    protected Pollution AddPollution(Vector2Int cell)
    {
        Pollution pollutionAtCell = GridMap.Current.GetObjectAtCell<Pollution>(cell, MapLayer.pollution);        
        bool pollutionAtTargetCell = pollutionAtCell != null;
        bool blockedByPriority = pollutionPrefab.PollutionData.Priority < (pollutionAtCell?.PollutionData?.Priority ?? 0);
        bool blockedByEffect = pollutionPrefab.PollutionData.IsBlockedByEffect(cell); //something other than isblocked?        
        if (pollutionAtTargetCell && !blockedByPriority && !blockedByEffect)
        {
            PollutionManager.Instance.RemovePollutionSoft(pollutionAtCell);            
        }        
        if(!blockedByPriority && !blockedByEffect)        
        {
            GameObject newGO = pollutionPool.GetGameObject();
            newGO.GetComponent<GridTransform>().MoveToMapCoords(cell);
            newGO.SetActive(true);
            Pollution newPollution = newGO.GetComponent<Pollution>();
            newPollution.pTypeController = this;
            pollutionObjects.Add(newPollution);
            newPollution.PollutionManager = this.pollutionManager;
            newPollution.SetAmount(newPollution.MaxAmount);
            OnPollutionAdd?.Invoke(cell, newPollution.Priority);
            return newPollution;
        }
        if(blockedByEffect && !blockedByPriority)
        {
            pollutionPrefab.PollutionData.TagBlockingEffect(cell);
        }
        return null;        
    }
}
