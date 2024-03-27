using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public class PollutionTypeController 
{   
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

    //let's store a disjoint set with deletions data structure for keeping track of the connected sets of pollution
    

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

        //bug: newly added pollution not getting proper color.
        //bug: colors flashing back and forth, no idea why...

        //problem is that initialization tries to access pgroups before we actually create pgroups...
        //probably addpollution should check if the dictionary element exists and then create it if not...

        //???
        //initialize the pollution groups...
        
        //this lookup should probably be the main data structure - groups are looked up by their ID in the connected components 
        
        foreach (var component in pollutionObjects.ConnectedComponents)
        {
            //i could somehow link the groups to the connected components but that feels VERY gross...
            //but uhh... why don't we just link the groups to the group IDs?  that makes more sense...
            
            foreach(Vector2Int pos in component.Value)
            {
                DebugTilemap.Instance.AddTile(pos, pGroups[component.Key].debugDisplayColor);
            }
        }


        //ISSUE -- appears to be the debug tile map, not the group ID.  possible to do w the dictionary that's in this class...

        //for each source, add it to the appropriate group (if the position has a group in the connected components... if not,
        //need a new group.





        //then... at some point we need to make sure to appropriately modify the colors when the components are joined and unjoined



        //add the connected components thing.  let's do a debug visualizer for this.

        //so...  we need to somehow identify connected components of pollution with the pollution sources that go with it.
        //so for one thing, we need to make pollution source - give it effects - make those effects transfer to pollution.

        //so maybe the algo is... generate the splotch map.
        //label connected components
        //generate a pollution source for each connectedc component.  assign each to one of the squares of a corresponding
        //connected component

        //doing this needs to add the effects of the pollution source to all pollutions of the connected component.  
        //this part may or may not be part of this initial function.
        //for example, we might have the effect contribution occur automatically as a result of adding the source
        //alternatively we might want to use custom functionality in this initializer to add the effects upon addition of 
        //connected component.
        //one option would be to, upon creation of pollution source, find associated connected component, and add the effects to all
        //that might be the best option - so let something else handle the effects.
        //map gen will just be... identify connected components, generate a source for each, add each source to map


        //then we need to determine the connected components,
        //then add the sources to each component
        //and, outside of this method, we need to control the pollutions to have the properties of the sources...
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
        if (!freePosAtCell && pollutionObjects.GetValue(cell) == null)//the second clause here is buggy...  
                                                                      //this whole thing is kind of f'ed...
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
        pGroups[groupID].sources.Add(psource);
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
