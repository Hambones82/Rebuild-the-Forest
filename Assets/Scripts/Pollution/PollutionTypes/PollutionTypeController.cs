using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PollutionTypeController //maybe make this generic for a pollution type???  not 100% sure...need to think abt it.
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
    [SerializeField]
    protected List<Pollution> pollutionObjects = new List<Pollution>();

    protected GameObjectPool pollutionPool;
    
    [SerializeField]
    private float spreadPeriod;
    private Timer spreadTimer;
    //private float spreadTimer = 0;

    [SerializeField]//a lower number means higher priority...
    private int priority;
    //this is a helper buffer to assist with another function.  maybe scope it to that function only
    private List<Pollution> workingPollutionObjects;

    [SerializeField]
    private int freeZoneWidth;

    public delegate void PollutionChangeDelegate(Vector2Int cell, int priority);
    public event PollutionChangeDelegate OnPollutionAdd;
    public event PollutionChangeDelegate OnPollutionDelete;


    //--INITIALIZATION METHODS--//
    public void Initialize(GridMap inGridMap, PollutionManager inPollutionManager)
    {
        gridMap = inGridMap;
        pollutionManager = inPollutionManager;
        pollutionPool = new GameObjectPool(pollutionPrefab.gameObject, parentObj: gridMap.gameObject, activeByDefault: false);
        priority = pollutionPrefab.PollutionData.Priority;
        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);
        freePositions = new List<Vector2Int>();
        spreadTimer = new Timer(spreadPeriod);
        spreadTimer.Enabled = true;
        spreadTimer.AddEvent(UpdatePollutionState);
    }

    public void InitializePollutionState()
    {        
        for (int x = 0; x < gridMap.width; x++)
        {
            AddFreePosition(new Vector2Int(x, 0));
            AddFreePosition(new Vector2Int(x, gridMap.height - 1));
        }
        for (int y = 1; y < gridMap.height - 1; y++)
        {
            AddFreePosition(new Vector2Int(0, y));
            AddFreePosition(new Vector2Int(gridMap.width - 1, y));
        }
        
        //full lines are from 0 to (gridMap.height - freeZoneWdith)/2
        //partial lines are from (gridMap.height - freeZoneWdith)/2 to (gridMap.height + freeZoneWdith)/2
        //partial lines have polution from 0 to (gridMap.width - freeZoneWdith)/2 and from (gridMap.width + freeZoneWdith)/2 to gridMap.width        

        for (int y = 0; y < gridMap.height; y++)
        {
            if(y < (gridMap.height - freeZoneWidth) / 2 || y > (gridMap.height + freeZoneWidth) / 2)
            {
                for (int x = 0; x < gridMap.width; x++)
                {
                    AddPollution(new Vector2Int(x, y));
                }
            }
            else
            {
                for (int x = 0; x < (gridMap.width - freeZoneWidth) / 2; x++)
                {
                    AddPollution(new Vector2Int(x, y));
                }
                for (int x = (gridMap.width + freeZoneWidth) / 2; x < gridMap.width; x++)
                {
                    AddPollution(new Vector2Int(x, y));
                }
            }            
        }
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
            if (!freePositions.Contains(position) && gridMap.IsWithinBounds(position) && !cellIsOccupied)//don't add if priority is wrong
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
        if (neighborsPollution) //also...  don't add it back if the priority thing is wrong... so... 
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
            //Debug.Log("removing free pos indicator");
            DebugTilemap.Instance.RemoveTile(cell);
        }

    }
    //--TIMING BASED METHODS FOR UPDATING POLLUTION STATE--//
    public void UpdateState()
    {
        spreadTimer.UpdateTimer();
    }

    protected void UpdatePollutionState()
    {
        if (freePositions.Count > 0)
        {
            int position = UnityEngine.Random.Range(0, freePositions.Count);
            AddPollution(freePositions[position]);
        }
    }

    //--METHODS FOR ADDING AND REMOVING POLLUTION, CALLED BY THE TIMING BASED UPDATES--//
    public virtual void RemovePollution(Pollution pollution, Vector2Int pollutionPosition)
    {
        if(pollution.pTypeController == this)//get rid of this -- it's definitely hurting performance.
        {
            pollutionObjects.Remove(pollution);
            pollutionPool.RecycleObject(pollution.gameObject);
            OnPollutionDelete?.Invoke(pollutionPosition, pollution.Priority);
        }
    }

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
            pollutionPrefab.PollutionData.NotifyBlockingEffectAt(cell);
        }
        return null;        
    }
}
