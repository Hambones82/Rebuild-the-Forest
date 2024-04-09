using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public HashSet<Vector2Int> freePositions;
        public Color debugDisplayColor;
        public float updateTimer;
        const float maxPeriodMultiplier = 4f;
        const int slowestSize = 50;

        public PollutionGroup()
        {            
            sources = new List<PollutionSource>();
            debugDisplayColor = UnityEngine.Random.ColorHSV();
            freePositions = new HashSet<Vector2Int>();
            updateTimer = 0;
        }

        public void Clear()
        {
            sources.Clear();
            freePositions.Clear();
        }
        public float GetUpdatePeriod(float basePeriod, int numPollutions)
        {
            //interpolate btw 1 and 50, use to select between 1 and max multiplier to modify base period
            float interpolationFactor = (numPollutions - 1) / (float)slowestSize;
            return Mathf.Lerp(basePeriod, maxPeriodMultiplier * basePeriod, interpolationFactor);
            //modify (slow down) base period with greater number of free positions?            
        }
        public List<Vector2Int> GetCandidateCellsToAddPollution()
        {
            List<Vector2Int> retval = new List<Vector2Int>();
            if (freePositions.Count == 0) return retval;
            int index = UnityEngine.Random.Range(0, freePositions.Count);
            retval.Add(freePositions.ElementAt(index));
            return retval;
        }
    }
    Dictionary<int, PollutionGroup> pGroups = new Dictionary<int, PollutionGroup>();
    [SerializeField]
    protected PollutionManager pollutionManager;

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
    private float baseSpreadPeriod;    
    private float rawSourceSpreadTimer = 0;

    [SerializeField]
    private int freeZoneWidth;

    [SerializeField]
    private List<PollutionSource> pollutionSources;

    [SerializeField]
    private List<PollutionEffect> sourceEffects;

    public void Update()
    {
        List<PollutionGroup> groups = pGroups.Select(item => item.Value).ToList();        
        foreach(var group in groups)
        {
            if (group.sources.Count == 0) continue;            
            group.updateTimer += Time.deltaTime;
            int numPollutions = pollutionObjects.GetPositions(group.collectionID).Count;
            float updatePeriod = group.GetUpdatePeriod(baseSpreadPeriod, numPollutions);
            if (group.updateTimer >= updatePeriod)
            {
                Debug.Log($"update period: {updatePeriod}, pollutions: {numPollutions}");
                group.updateTimer -= updatePeriod;
                List<Vector2Int> candidateCellsToAdd = group.GetCandidateCellsToAddPollution();
                List<Vector2Int> confirmedCellsToAdd = new List<Vector2Int>();
                foreach (Vector2Int candidateCell in candidateCellsToAdd)
                {
                    if (!BlockedByEffect(candidateCell))
                    {
                        confirmedCellsToAdd.Add(candidateCell);
                    }
                    else
                    {
                        PollutionPrefab.OnSpawnBlocked(candidateCell);
                    }
                }
                foreach (Vector2Int cell in confirmedCellsToAdd)
                {
                    pollutionManager.AddPollution(cell, this);
                    pollutionManager.UpdateFreePositionsForAddition(cell);
                }
            }                        
        }
        List<PollutionSource> rawSources = new List<PollutionSource>();
        foreach(PollutionSource source in pollutionSources)
        {
            Vector2Int sourcePos = source.GetComponent<GridTransform>().topLeftPosMap;
            if(pollutionObjects.GetGraphID(sourcePos) == -1)
            {
                rawSources.Add(source);
            }
        }
        if(rawSources.Count > 0)
        {
            rawSourceSpreadTimer += Time.deltaTime;
            if(rawSourceSpreadTimer >= baseSpreadPeriod)
            {
                rawSourceSpreadTimer -= baseSpreadPeriod;
                //select one, add to it
                int selectionIndex = UnityEngine.Random.Range(0, rawSources.Count);
                AddPollution(rawSources[selectionIndex].GetComponent<GridTransform>().topLeftPosMap);
            }
        }                        
    }


    public IReadOnlyList<PollutionEffect> GetSourceEffectsAt(Vector2Int cell)
    {
        List<PollutionEffect> retval = new List<PollutionEffect>();
        int graphID = GetGraphID(cell);
        foreach(PollutionSource psource in pGroups[graphID].sources)
        {
            retval.AddRange(psource.Effects);
        }
        return retval;
    }
    public int GetGraphID(Vector2Int cell)
    {
        if (pollutionObjects == null) Debug.Log("somehow, pollution objects is null");        
        return pollutionObjects.GetGraphID(cell);
    }

    //--INITIALIZATION METHODS--//
    public void Initialize(GridMap inGridMap, PollutionManager inPollutionManager)
    {
        gridMap = inGridMap;
        pollutionManager = inPollutionManager;                
        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);          
        pollutionObjects = new Vector2GraphSet<Pollution>(gridMap.width, gridMap.height);
        pollutionSources = new List<PollutionSource>();
        pGroups = new Dictionary<int, PollutionGroup>();
        pollutionPool = new GameObjectPool(pollutionPrefab.gameObject, parentObj: gridMap.gameObject, activeByDefault: false);
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
            minSplotchRadius = 3,
            maxSplotchRadius = 5,
            //splotchProbability = .8f,
            //splotchProbability = 0f,
            splotchProbability = .5f,
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
            
            else
            {
                existing.PTypeController.RemovePollution(existing, cell);
                AddPollution(cell);
            }
            
            if (splotchMap.splotchValues[cell.x, cell.y] == SplotchMap.SplotchValue.SplotchCenter)
            {                
                PollutionSource pollutionSource = 
                    GameObject.Instantiate(pollutionSourcePrefab, GridMap.Current.transform);
                pollutionSource.GetComponent<GridTransform>().MoveToMapCoords(cell);
                pollutionSources.Add(pollutionSource);
                // ADD A RANDOM EFFECT TO THE SOURCE
                if(pollutionSources.Count > 0)
                {
                    int numEffects = sourceEffects.Count;
                    int randomIndex = UnityEngine.Random.Range(0, numEffects);
                    pollutionSource.AddEffect(sourceEffects.ElementAt(randomIndex));
                }                
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
        foreach(var component in pGroups)
        {
            UpdatePGroupProperties(component.Key);
        }        
    }

    public void RecalculateFreePositions(int groupID)
    {        
        //check the pollution sources...
        var positions = pollutionObjects.GetPositions(groupID);
        pGroups[groupID].freePositions.Clear();
        //also clear the debug tiles?
        foreach(Vector2Int position in positions)
        {
            List<Vector2Int> neighbors = position.GetNeighborsInBounds(gridMap.width, gridMap.height);
            foreach(Vector2Int neighbor in neighbors)
            {
                if(pollutionObjects.GetValue(neighbor) == null)
                {
                    pGroups[groupID].freePositions.Add(neighbor);
                }
            }
        }
        //set the debug tiles.
    }

   //add free position to all neighbor groups of the cell
    public void RecalculateFreePositions()//... i think we should do this for ALL free positions.
    {

        for(int x = 0; x < gridMap.width; x++)
        {
            for(int y = 0; y < gridMap.height; y++)
            {
                RemoveFreePosition(new Vector2Int(x, y));
            }
        }
        
        for (int x = 0; x < gridMap.width; x++)
        {
            for (int y = 0; y < gridMap.height; y++)
            {
                Vector2Int cell = new Vector2Int(x, y);
                Pollution pollution = GetPollutionAt(cell);

                if (pollution != null) continue;

                List<Vector2Int> neighbors = cell.GetNeighbors();
                bool isFree = false;
                foreach (Vector2Int neighbor in neighbors)
                {
                    if (GridMap.Current.IsWithinBounds(neighbor))
                    {
                        isFree |= GetPollutionAt(neighbor) != null;
                    }
                }
                if (isFree)
                {
                    AddFreePosition(cell);  //i think we need to just have this for each group id rather than a universal one                  
                }
            }
        }
    }

    public Pollution GetPollutionAt(Vector2Int cell)
    {
        return GridMap.Current.GetObjectAtCell<Pollution>(cell, MapLayer.pollution)?.GetComponent<Pollution>(); 
    }

    //--METHODS FOR TRACKING WHAT POSITIONS IN THE MAP ARE AVAILABLE FOR NEXT POLLUTION SPAWN--//
    public void UpdateFreePositionsForPollutionAddition(Vector2Int cell)
    {             
        RemoveFreePosition(cell);
        
        List<Vector2Int> positions = cell.GetNeighborsInBounds(gridMap.width, gridMap.height);

        foreach (Vector2Int position in positions)
        {
            Pollution pollAtCell = GridMap.Current.GetObjectAtCell<Pollution>(position, MapLayer.pollution)?.GetComponent<Pollution>();            
            
            if (pollAtCell == null)
            {
                AddFreePosition(position);
            }
        }
    }

    public void UpdateFreePositionsForPollutionRemoval(Vector2Int cell)
    {                
        List<Vector2Int> neighbors = cell.GetNeighborsInBounds(gridMap.width, gridMap.height);
        bool neighborsPollution = false;
        foreach (Vector2Int neighborPosition in neighbors)
        {
            if (pollutionMap.IsCellOccupied(neighborPosition))
            {                
                neighborsPollution = true;                
            }
            List<Vector2Int> subPositions = neighborPosition.GetNeighborsInBounds(gridMap.width, gridMap.height);
            bool subNeighborsPollution = false;
            
            foreach (Vector2Int subPosition in subPositions)
            {
                if (pollutionMap.IsCellOccupied(subPosition))
                {                        
                    subNeighborsPollution = true;                        
                }
            }
            if (subNeighborsPollution == false)
            {
                RemoveFreePosition(neighborPosition);
            }
            
        }
        if (neighborsPollution) 
        {
            AddFreePosition(cell); 
        }
    }

    private void AddFreePosition(Vector2Int cell)
    {
        foreach(Vector2Int neighbor in cell.GetNeighborsInBounds(gridMap.width, gridMap.height))
        {
            int neighborID = pollutionObjects.GetGraphID(neighbor);
            if (neighborID != -1)
            {
                pGroups[neighborID].freePositions.Add(cell);
            }            
        }        
        DebugTilemap.Instance.AddTile(cell);
    }

    
    private void RemoveFreePosition(Vector2Int cell)
    {
        foreach (Vector2Int neighbor in cell.GetNeighborsInBounds(gridMap.width, gridMap.height))
        {
            int neighborID = pollutionObjects.GetGraphID(neighbor);
            if(neighborID != -1)
            {
                pGroups[neighborID].freePositions.Remove(cell);
            }            
        }

        if (pollutionObjects.GetValue(cell) == null)                                                    
        {
            DebugTilemap.Instance.RemoveTile(cell);
        }

    }

    public List<Vector2Int> GetAllFreePositions()
    {
        List<Vector2Int> retval = new List<Vector2Int>();
        foreach(var pgroupKVP in pGroups)
        {
            retval.AddRange(pgroupKVP.Value.freePositions);
        }
        int i = 0;
        foreach(var psource in pollutionSources)
        {
            Vector2Int psourcePos = psource.GetComponent<GridTransform>().topLeftPosMap;
            if (pollutionObjects.GetGraphID(psourcePos) == -1)
            {
                //Debug.Log($"adding raw psource {psourcePos}");
                retval.Add(psourcePos);
                i++;
            }
        }
        //Debug.Log($"added {i} raw psources");
        return retval;
    }
    //--TIMING BASED METHODS FOR UPDATING POLLUTION STATE--//
   
    public List<Vector2Int> GetCandidateCellsToAddPollution()
    {

        var freePositions = GetAllFreePositions();//pGroups[pGroupID].freePositions.ToList();
        
        int positionIndex = UnityEngine.Random.Range(0, freePositions.Count);
        List<Vector2Int> retval = new List<Vector2Int>();        

        if (freePositions.Count == 0)
        {
            return retval;
        }
        try
        {
            retval.Add(freePositions[positionIndex]); //??????????
        }
        catch (System.Exception e)
        {
            Debug.Log($"trying to add for free positions {positionIndex}");
            throw e;
        }
        return retval;
    }


    //--METHODS FOR ADDING AND REMOVING POLLUTION, CALLED BY THE TIMING BASED UPDATES--//
    public void RemovePollution(Pollution pollution, Vector2Int cell)
    {
        if(pollution.PTypeController == this)
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
    //maybe recalculate free positions for each pgroup id only (not whole map).

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
            //apply the effects of the source to the pollutions in the group
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
        
        else
        {
            RecalculateFreePositions(pGroupID);
        }
        //RecalculateFreePositions(); //this results in high initialization lag because it happens for every addition, and half 
        //of the map is additions, resulting in n^2 time
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
        //also... if the group has no more sources, remove all free positions.
    }

    private void RemoveSourceFromGroup(PollutionSource psource, int groupID)
    {
        pGroups[groupID].sources.Remove(psource);
    }

    private void AddSourceToGroup(PollutionSource psource, int groupID)
    {
        List<PollutionSource> sources = pGroups[groupID].sources;
        List<PollutionSource> addedSources = new List<PollutionSource>();
        if(!sources.Contains(psource))
        {
            pGroups[groupID].sources.Add(psource);
            addedSources.Add(psource);
        }
        /*//can we just have the pollution controller (the pollution?) iterate through all of the connected psources???
        foreach(var source in addedSources)
        {
            foreach(PollutionEffect effect in source.Effects)
            {
                //add that effect to all pollutions of the group, if that group does not already contain that effect
            }
        }
        */
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
    
    public Pollution AddPollution(Vector2Int cell)
    {        
        GameObject newGO = pollutionPool.GetGameObject();
        newGO.GetComponent<GridTransform>().MoveToMapCoords(cell);
        newGO.SetActive(true);
        Pollution newPollution = newGO.GetComponent<Pollution>();
        newPollution.PTypeController = this;
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

    public string DebugText(Vector2Int cell)
    {
        string retval = "";
        foreach(var pgroupKVP in pGroups)
        {
            if(pgroupKVP.Value.freePositions.Contains(cell))
            {
                retval += $"Group {pgroupKVP.Key} contains {cell}\n";
            }
        }
        return retval;
    }
}
