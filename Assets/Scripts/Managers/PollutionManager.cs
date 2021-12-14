using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//this is done in a very naive way, could be optimized... not necessary for prototype
[DefaultExecutionOrder(-3)]
public class PollutionManager : MonoBehaviour
{
    public delegate void PollutionEvent(Vector2Int cell);
    //public event 
    //public delegate void PollutionCompleteEvent();
    public event Action OnInitComplete;
    public event PollutionEvent OnPollutionDead;

    private static PollutionManager _instance;
    public static PollutionManager Instance
    {
        get => _instance;
    }

    [SerializeField]
    private GridMap gridMap;
    [SerializeField]
    private GridSubMap pollutionMap;
    [SerializeField]
    private Pollution pollutionPrefab;
    [SerializeField]
    private List<Pollution> pollutionObjects = new List<Pollution>();

    [SerializeField]
    private MapEffectType treeBlockEffect;
    [SerializeField]
    private MapEffectType plantBlockEffect;
    [SerializeField]
    private MapEffectType mushroomBlockEffect;

    //this is a helper buffer to assist with another function.  maybe scope it to that function only
    private List<Pollution> workingPollutionObjects;

    [SerializeField, Range(0, 1)]
    private float spreadBaseProbability;
    [SerializeField]
    private float spreadPeriod;
    private float spreadTimer = 0;

    private GameObjectPool pollutionPool;
    
    private List<Vector2Int> freePositions;

    private void Awake()
    {
        pollutionPool = new GameObjectPool(pollutionPrefab.gameObject, parentObj: gridMap.gameObject, activeByDefault: false);
        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);
        
        freePositions = new List<Vector2Int>();

        bool initialMapHasPollution = false;

        foreach (GridTransform gt in pollutionMap.GetAllObjects())
        {
            initialMapHasPollution = true;
            Pollution pToAdd = gt.GetComponent<Pollution>();
            pToAdd.PollutionManager = this;
            pollutionObjects.Add(pToAdd);
            UpdateFreePositionsForAddition(pToAdd.GetComponent<GridTransform>().topLeftPosMap);
        }

        if(!initialMapHasPollution)
        {
            for(int x = 0; x < gridMap.width; x++)
            {
                AddFreePosition(new Vector2Int(x, 0));
                AddFreePosition(new Vector2Int(x, gridMap.height - 1));
            }
            for(int y = 1; y < gridMap.height - 1; y++)
            {
                AddFreePosition(new Vector2Int(0, y));
                AddFreePosition(new Vector2Int(gridMap.width - 1, y));
            }
        }
        PopulateInitialPollution();
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            throw new InvalidOperationException("can't have two pollution managers");
        }
    }


    private void Start()
    {
        OnInitComplete?.Invoke();
    }

    private void UpdateFreePositionsForAddition(Vector2Int cell)
    {
        if(freePositions.Contains(cell))
        {
            RemoveFreePosition(cell);
        }

        List<Vector2Int> positions = cell.GetNeighbors();

        foreach(Vector2Int position in positions)
        {
            if (!freePositions.Contains(position) && gridMap.IsWithinBounds(position) && !pollutionMap.IsCellOccupied(position))
            {
                AddFreePosition(position);
            }
        }

        //Debug.Log($"after addition: {freePositions.Count} # of free positions");
    }

    private void UpdateFreePositionsForRemoval(Vector2Int cell)
    {
        //get all neighbors
        //check whether they still neighbor any pollution
        //remove if not
        List<Vector2Int> positions = cell.GetNeighbors();
        bool neighborsPollution = false;
        foreach(Vector2Int position in positions)
        {
            if(pollutionMap.IsCellOccupied(position))
            {
                neighborsPollution = true;
            }
            List<Vector2Int> subPositions = position.GetNeighbors();
            bool subNeighborsPollution = false;
            if(freePositions.Contains(position))
            {
                foreach (Vector2Int subPosition in subPositions)
                {
                    if (pollutionMap.IsCellOccupied(subPosition))
                    {
                        subNeighborsPollution = true;
                    }
                }
                if (subNeighborsPollution == false)
                {
                    RemoveFreePosition(position);
                }
            }
        }
        if(neighborsPollution)
        {
            AddFreePosition(cell);
        }
        //Debug.Log($"after removal: {freePositions.Count} # of free positions");
    }

    [SerializeField]
    private int freeZoneWidth;

    private void PopulateInitialPollution()
    {
        for(int y = 0; y < gridMap.height; y++)
        {
            for(int x = 0; x < gridMap.width; x++)
            {
                //left half: from 0 to width - freezone / 2
                bool spawnHere = true;
                int minMiddleX = (gridMap.width - freeZoneWidth) / 2;
                int maxMiddleX = (gridMap.width + freeZoneWidth) / 2;
                int minMiddleY = (gridMap.height - freeZoneWidth) / 2;
                int maxMiddleY = (gridMap.height + freeZoneWidth) / 2;
                
                if(x > minMiddleX && x < maxMiddleX && y > minMiddleY && y < maxMiddleY)
                {
                    spawnHere = false;
                }
                if(spawnHere)
                {
                    AddPollution(new Vector2Int(x, y));
                }
            }
        }
    }
    
    private void Update()
    {
        spreadTimer += Time.deltaTime;
        if(spreadTimer >= spreadPeriod)
        {
            spreadTimer = 0;
            UpdatePollutionState();
        }

    }

    //fix this.
    //one thing we could do is just random... i mean instead of looking through all pollution objects, just get a random one.  try to put adjacent to that.
    //problem is there would be a lot of no's.  
    //another possible solution is keep a list of free spaces and select one of those randomly, possibly testing for adjacency.  
    //i think we just need something simple.  
    //if map is not just go through the list of empty cells randomly.  check 
    private void UpdatePollutionState()
    {
        if(freePositions.Count > 0)
        {
            //Debug.Log($"free position count: {freePositions.Count}");
            int position = UnityEngine.Random.Range(0, freePositions.Count);
            AddPollution(freePositions[position]);
        }
    }
    
    //this needs the checker -- whether there's a block pollution effect...
    private Pollution AddPollution(Vector2Int cell)
    {
        bool addPollution = true;
        List<MapEffectObject> effectsAtCell = MapEffectsManager.Instance.GetEffectsAtCell(cell);
        //could put the code below into a function in mapeffectsmanager.
        if (effectsAtCell != null)
        {
            bool mush = false;
            bool plant = false;
            bool tree = false;
            foreach (MapEffectObject effectObject in effectsAtCell)
            {
                if (effectObject.EffectType == treeBlockEffect)
                {
                    tree = true;
                }
                else if (effectObject.EffectType == mushroomBlockEffect)
                {
                    mush = true;
                }
                else if (effectObject.EffectType == plantBlockEffect)
                {
                    plant = true;
                }
            }
            addPollution = !(mush && plant && tree);
        }

        if(addPollution)
        {
            GameObject newGO = pollutionPool.GetGameObject();
            newGO.SetActive(true);
            newGO.GetComponent<GridTransform>().MoveToMapCoords(cell);
            Pollution newPollution = newGO.GetComponent<Pollution>();
            pollutionObjects.Add(newPollution);
            newPollution.PollutionManager = this;
            //freePositions.Remove(cell);
            newPollution.SetAmount(newPollution.MaxAmount);
            UpdateFreePositionsForAddition(cell);
            return newPollution;
        }
        else
        {
            return null;
        }
    }

    public void RemovePollution(Pollution pollution)
    {
        Vector2Int pollutionPosition = pollution.GetComponent<GridTransform>().topLeftPosMap;
        pollutionObjects.Remove(pollution);
        pollutionPool.RecycleObject(pollution.gameObject);
        //freePositions.Add(pollutionPosition);
        UpdateFreePositionsForRemoval(pollutionPosition);
        OnPollutionDead(pollutionPosition);
    }


    private void AddFreePosition(Vector2Int cell)
    {
        freePositions.Add(cell);
        //if (DebugTilemap.Instance == null) Debug.Log("there is a problem!");
        DebugTilemap.Instance.AddTile(cell);
    }

    private void RemoveFreePosition(Vector2Int cell)
    {
        freePositions.Remove(cell);
        DebugTilemap.Instance.RemoveTile(cell);
    }
   
}
