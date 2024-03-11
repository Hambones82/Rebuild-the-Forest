using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public class PollutionTypeController 
{    
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
    public Pollution PollutionPrefab { get => pollutionPrefab; }
    


    [SerializeField]
    protected List<Pollution> pollutionObjects = new List<Pollution>();

    protected GameObjectPool pollutionPool;
    
    [SerializeField]
    private float spreadPeriod;    
    private float spreadTimer = 0;

    [SerializeField]//a lower number means higher priority...
    private int priority;
    public int Priority { get => priority; }    

    [SerializeField]
    private int freeZoneWidth;

    
    //--INITIALIZATION METHODS--//
    public void Initialize(GridMap inGridMap, PollutionManager inPollutionManager)
    {
        gridMap = inGridMap;
        pollutionManager = inPollutionManager;
        pollutionPool = new GameObjectPool(pollutionPrefab.gameObject, parentObj: gridMap.gameObject, activeByDefault: false);
        priority = pollutionPrefab.PollutionData.Priority;
        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);
        freePositions = new List<Vector2Int>();        
    }

    //let's store a disjoint set with deletions data structure for keeping track of the connected sets of pollution
    

    public void InitializePollutionState()
    {
        List<Vector2Int> pollutionsToAdd = new List<Vector2Int>();
        for (int y = 0; y < gridMap.height; y++)
        {
            if(y < (gridMap.height - freeZoneWidth) / 2 || y > (gridMap.height + freeZoneWidth) / 2)
            {
                for (int x = 0; x < gridMap.width; x++)
                {
                    pollutionsToAdd.Add(new Vector2Int(x, y));
                }
            }
            else
            {
                for (int x = 0; x < (gridMap.width - freeZoneWidth) / 2; x++)
                {
                    pollutionsToAdd.Add(new Vector2Int(x, y));
                }
                for (int x = (gridMap.width + freeZoneWidth) / 2; x < gridMap.width; x++)
                {
                    pollutionsToAdd.Add(new Vector2Int(x, y));
                }
            }            
        }
        
        foreach(Vector2Int cell in pollutionsToAdd)
        {
            Pollution existing = GetPollutionAt(cell);
            if(existing == null)
            {                
                AddPollution(cell);
                continue;
            }
            
            if(existing.Priority < priority)
            {
                existing.pTypeController.RemovePollution(existing);
                AddPollution(cell);        
            }            
        }
    }

    public void RecalculateFreePositions()
    {   
        for(int i = freePositions.Count - 1; i >= 0; i--)
        {
            RemoveFreePosition(freePositions[i]);
        }     
        
        for(int x = 0; x < gridMap.width; x++)
        {
            for(int y = 0; y < gridMap.height; y++)
            {                
                Vector2Int cell = new Vector2Int(x, y);
                Pollution pollution = GetPollutionAt(cell);
                if(pollution != null)
                {
                    if (pollution.Priority >= priority)
                    {
                        continue;
                    }
                }                
                List<Vector2Int> neighbors = cell.GetNeighbors();
                bool isFree = false;
                foreach(Vector2Int neighbor in neighbors)
                {
                    if(GridMap.Current.IsWithinBounds(neighbor))
                    {   
                        isFree |= GetPollutionAt(neighbor)?.Priority == priority;
                    }
                }
                if(isFree)
                {                    
                    AddFreePosition(cell);                    
                }
            }
        }
    }

    public Pollution GetPollutionAt(Vector2Int cell)
    {
        return GridMap.Current.GetObjectAtCell<Pollution>(cell, MapLayer.pollution)?.GetComponent<Pollution>(); 
    }

    //--METHODS FOR TRACKING WHAT POSITIONS IN THE MAP ARE AVAILABLE FOR NEXT POLLUTION SPAWN--//
    public void UpdateFreePositionsForPollutionAddition(Vector2Int cell, int inPriority)
    {
        if (inPriority < priority) return;
        if (freePositions.Contains(cell))
        {
            RemoveFreePosition(cell);
        }

        List<Vector2Int> positions = cell.GetNeighbors();

        foreach (Vector2Int position in positions)
        {
            Pollution pollAtCell = GridMap.Current.GetObjectAtCell<Pollution>(position, MapLayer.pollution)?.GetComponent<Pollution>();
            bool cellIsOccupied = false;
            if (pollAtCell != null)
            {
                cellIsOccupied = pollAtCell.Priority >= priority;
            }
            if (!freePositions.Contains(position) && gridMap.IsWithinBounds(position) && !cellIsOccupied)
            {
                AddFreePosition(position);
            }
        }
    }

    public void UpdateFreePositionsForPollutionRemoval(Vector2Int cell, int inPriority)
    {        
        if (inPriority < priority)
        {
            return;
        }

        List<Vector2Int> positions = cell.GetNeighbors();
        bool neighborsPollution = false;
        foreach (Vector2Int position in positions)
        {
            if (pollutionMap.IsCellOccupied(position))
            {
                if (GridMap.Current.GetObjectAtCell<Pollution>(position, MapLayer.pollution).Priority == priority)
                {
                    neighborsPollution = true;
                }
            }
            List<Vector2Int> subPositions = position.GetNeighbors();
            bool subNeighborsPollution = false;
            if (freePositions.Contains(position))
            {
                foreach (Vector2Int subPosition in subPositions)
                {
                    if (pollutionMap.IsCellOccupied(subPosition))
                    {
                        if (GridMap.Current.GetObjectAtCell<Pollution>(subPosition, MapLayer.pollution).Priority == priority)
                        {
                            subNeighborsPollution = true;
                        }
                    }
                }
                if (subNeighborsPollution == false)
                {
                    RemoveFreePosition(position);
                }
            }
        }
        if (neighborsPollution) 
        {
            AddFreePosition(cell); 
        }
    }

    private void AddFreePosition(Vector2Int cell)
    {
        freePositions.Add(cell);
        DebugTilemap.Instance.AddTile(cell);
    }

    
    private void RemoveFreePosition(Vector2Int cell)
    {
        freePositions.Remove(cell);
        bool freePosAtCell = false;
        foreach (PollutionTypeController controller in PollutionManager.Instance.PollutionControllers)
        {
            if (controller.FreePositions.Contains(cell)) freePosAtCell = true;
        }
        if (!freePosAtCell)
        {
            DebugTilemap.Instance.RemoveTile(cell);
        }

    }
    //--TIMING BASED METHODS FOR UPDATING POLLUTION STATE--//
   
    public bool CheckForTick()
    {
        spreadTimer += Time.deltaTime;
        if (spreadTimer >= spreadPeriod)
        {
            spreadTimer -= spreadPeriod;
            return true;
        }
        return false;
    }

    public List<Vector2Int> GetCandidateCellsToAddPollution()
    {        
        int position = UnityEngine.Random.Range(0, freePositions.Count - 1);
        List<Vector2Int> retval = new List<Vector2Int>();
        if (freePositions.Count == 0)
        {
            return retval;
        }
        try
        {
            retval.Add(freePositions[position]);
        }
        catch (System.Exception e)
        {
            Debug.Log($"trying to add for free positions {position}");
            throw e;
        }
        return retval;
    }


    //--METHODS FOR ADDING AND REMOVING POLLUTION, CALLED BY THE TIMING BASED UPDATES--//
    public void RemovePollution(Pollution pollution)
    {
        if(pollution.pTypeController == this)
        {
            pollutionObjects.Remove(pollution);
            pollutionPool.RecycleObject(pollution.gameObject);            
        }
    }

    public bool BlockedByEffect(Vector2Int cell)
    {
        return pollutionPrefab.PollutionData.IsBlockedByEffect(cell);
    }

    public bool BlockedByPriorityOf(Pollution pollutionAtCell)
    {        
        return pollutionPrefab.PollutionData.Priority < (pollutionAtCell?.PollutionData?.Priority ?? 0);
    }
    
    public Pollution AddPollution(Vector2Int cell)
    {        
        GameObject newGO = pollutionPool.GetGameObject();
        newGO.GetComponent<GridTransform>().MoveToMapCoords(cell);
        newGO.SetActive(true);
        Pollution newPollution = newGO.GetComponent<Pollution>();
        newPollution.pTypeController = this;
        pollutionObjects.Add(newPollution);
        newPollution.PollutionManager = this.pollutionManager;
        newPollution.SetAmount(newPollution.MaxAmount);        
            
        return newPollution;        
    }
}
