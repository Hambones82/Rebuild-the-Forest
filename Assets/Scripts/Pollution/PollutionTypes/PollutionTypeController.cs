using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public class PollutionTypeController 
{
    [System.Serializable]
    private class PollutionGroup
    {
        public int collectionID;
        public List<PollutionSource> sources;
        public Color debugDisplayColor;
        public PollutionGroup()
        {            
            sources = new List<PollutionSource>();
            debugDisplayColor = UnityEngine.Random.ColorHSV();
        }
    }
    Dictionary<int, PollutionGroup> pGroups = new Dictionary<int, PollutionGroup>();
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
    protected PollutionSource pollutionSourcePrefab;
    public PollutionSource PollutionSourcePrefab { get => pollutionSourcePrefab; }


    //[SerializeField]
    //protected List<Pollution> pollutionObjects = new List<Pollution>();
    private Vector2GraphSet<Pollution> pollutionObjects;    

    protected GameObjectPool pollutionPool;
    
    [SerializeField]
    private float spreadPeriod;    
    private float spreadTimer = 0;

    [SerializeField]//a lower number means higher priority...
    private int priority;
    public int Priority { get => priority; }    

    [SerializeField]
    private int freeZoneWidth;

    [SerializeField]
    private List<PollutionSource> pollutionSources;

    
    public int GetGraphID(Vector2Int cell)
    {
        return pollutionObjects.GetGraphID(cell);
    }

    //--INITIALIZATION METHODS--//
    public void Initialize(GridMap inGridMap, PollutionManager inPollutionManager)
    {
        gridMap = inGridMap;
        pollutionManager = inPollutionManager;
        pollutionPool = new GameObjectPool(pollutionPrefab.gameObject, parentObj: gridMap.gameObject, activeByDefault: false);
        priority = pollutionPrefab.PollutionData.Priority;
        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);
        freePositions = new List<Vector2Int>();  
        pollutionObjects = new Vector2GraphSet<Pollution>(gridMap.width, gridMap.height);
        pollutionSources = new List<PollutionSource>();
        pGroups = new Dictionary<int, PollutionGroup>();
    }

    public void InitializePollutionState()
    {
        List<Vector2Int> pollutionsToAdd = new List<Vector2Int>();
        SplotchParameters splotchParameters = new SplotchParameters
        {
            width = (uint)GridMap.Current.width,
            height = (uint)GridMap.Current.height,
            numCellsHorizontal = 5,
            numCellsVertical = 5,
            minSplotchRadius = 4,
            maxSplotchRadius = 7,
            splotchProbability = .8f,
            seed = (uint)(Random.Range(0f, 1f) * 10000)
        };
        SplotchMap splotchMap = SplotchGenerator.GenerateSplotchMap(splotchParameters);
        for (int y = 0; y < gridMap.height; y++)
        {
            if(y < (gridMap.height - freeZoneWidth) / 2 || y > (gridMap.height + freeZoneWidth) / 2)
            {
                for (int x = 0; x < gridMap.width; x++)
                {
                    SplotchMap.SplotchValue splotchValue = splotchMap.splotchValues[x, y];
                    if (splotchValue != SplotchMap.SplotchValue.Empty)
                    {
                        pollutionsToAdd.Add(new Vector2Int(x, y));
                    }                        
                }
            }
            else
            {
                for (int x = 0; x < (gridMap.width - freeZoneWidth) / 2; x++)
                {
                    SplotchMap.SplotchValue splotchValue = splotchMap.splotchValues[x, y];
                    if (splotchValue != SplotchMap.SplotchValue.Empty)
                    {
                        pollutionsToAdd.Add(new Vector2Int(x, y));

                    }
                        
                }
                for (int x = (gridMap.width + freeZoneWidth) / 2; x < gridMap.width; x++)
                {
                    SplotchMap.SplotchValue splotchValue = splotchMap.splotchValues[x, y];
                    if (splotchValue != SplotchMap.SplotchValue.Empty)
                    {
                        pollutionsToAdd.Add(new Vector2Int(x, y));
                    }
                        
                }
            }            
        }
        
        foreach(Vector2Int cell in pollutionsToAdd)
        {
            Pollution existing = GetPollutionAt(cell);
            if(existing == null)
            {                
                AddPollution(cell);                
            }
            
            else if(existing.Priority < priority)
            {
                existing.pTypeController.RemovePollution(existing, cell);
                AddPollution(cell);
            }
            
            if (splotchMap.splotchValues[cell.x, cell.y] == SplotchMap.SplotchValue.SplotchCenter)
            {                
                PollutionSource pollutionSource = 
                    GameObject.Instantiate(pollutionSourcePrefab, GridMap.Current.transform);
                pollutionSource.GetComponent<GridTransform>().MoveToMapCoords(cell);
                pollutionSources.Add(pollutionSource);
                //also add to pgroup.sources???
            }
            
        }

        
        foreach (var component in pollutionObjects.ConnectedComponents)
        {
            //i could somehow link the groups to the connected components but that feels VERY gross...
            //but uhh... why don't we just link the groups to the group IDs?  that makes more sense...
            
            foreach(Vector2Int pos in component.Value)
            {
                DebugTilemap.Instance.AddTile(pos, pGroups[component.Key].debugDisplayColor);
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
        
        if (pollutionObjects.GetValue(cell) == null)                                                    
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
    public void RemovePollution(Pollution pollution, Vector2Int cell)
    {
        if(pollution.pTypeController == this)
        {
            pollutionObjects.RemoveValue(cell);            
            pollutionPool.RecycleObject(pollution.gameObject);
            DebugTilemap.Instance.RemoveTile(cell);
            foreach(int pGroupID in pollutionObjects.DirtyIDs)
            {
                UpdatePGroupProperties(pGroupID);
            }
            pollutionObjects.ClearDirtyIDs();
        }
    }

    private void UpdatePGroupProperties(int pGroupID)
    {
        PollutionGroup pgroup;
        if(!pGroups.TryGetValue(pGroupID, out var pGroup))
        {
            pgroup = new PollutionGroup();
            pgroup.collectionID = pGroupID;
            pGroups.Add(pGroupID, pgroup);
        }
        else
        {
            pgroup = pGroups[pGroupID];
        }        
        List<Vector2Int> cells = pollutionObjects.ConnectedComponents[pgroup.collectionID];
        

        //maybe... find all the sources that are affected, adjust as appropriate?
        List<PollutionSource> sourcesThatShouldBeInPGroup = new List<PollutionSource>();
        List<PollutionSource> sourcesThatShouldBeRemovedFromPGroup = new List<PollutionSource>();
        foreach (PollutionSource psource in pollutionSources)
        {
            Vector2Int psourcePos = psource.GetComponent<GridTransform>().topLeftPosMap;
            if(pollutionObjects.GetGraphID(psourcePos) == pGroupID)
            {
                sourcesThatShouldBeInPGroup.Add(psource);
            }
            else if (pGroups[pGroupID].sources.Contains(psource))
            {
                sourcesThatShouldBeRemovedFromPGroup.Add(psource);
            }
        }
        foreach(PollutionSource psource in sourcesThatShouldBeInPGroup)
        {
            AddSourceToGroup(psource, pGroupID);
        }
        foreach(PollutionSource psource in sourcesThatShouldBeRemovedFromPGroup)
        {
            RemoveSourceFromGroup(psource, pGroupID);
        }

        foreach (Vector2Int cell in cells)
        {
            DebugTilemap.Instance.AddTile(cell, pGroups[pGroupID].debugDisplayColor);
        }

        if (cells.Count == 0)
        {
            pGroups.Remove(pGroupID);            
        }
    }

    public void NotifyOfSourceDeletion(PollutionSource psource)
    {
        Vector2Int psourcePos = psource.GetComponent<GridTransform>().topLeftPosMap;
        foreach(var group in pGroups)
        {
            if(group.Value.sources.Contains(psource))
            {
                RemoveSourceFromGroup(psource, group.Key);
            }
        }
        pollutionSources.Remove(psource);
    }

    private void RemoveSourceFromGroup(PollutionSource psource, int groupID)
    {
        pGroups[groupID].sources.Remove(psource);
    }

    private void AddSourceToGroup(PollutionSource psource, int groupID)
    {
        List<PollutionSource> sources = pGroups[groupID].sources;
        if(!sources.Contains(psource))
        {
            pGroups[groupID].sources.Add(psource);
        }        
    }

    public int GetPSourceGroupID(PollutionSource psource)
    {
        foreach(var p in pGroups)
        {
            if(p.Value.sources.Contains(psource)) return p.Key;
        }
        return -1;
    }
    
    public bool BlockedByEffect(Vector2Int cell)
    {        
        return pollutionPrefab.IsSpawnBlocked(cell);
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
        pollutionObjects.AddValue(newPollution, cell);
        newPollution.PollutionManager = this.pollutionManager;
        newPollution.SetAmount(newPollution.MaxAmount);

        foreach (int pGroupID in pollutionObjects.DirtyIDs)
        {
            UpdatePGroupProperties(pGroupID);
        }
        pollutionObjects.ClearDirtyIDs();

        return newPollution;        
    }
}
