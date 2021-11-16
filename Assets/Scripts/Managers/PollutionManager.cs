using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//this is done in a very naive way, could be optimized... not necessary for prototype
public class PollutionManager : MonoBehaviour
{
    [SerializeField]
    private GridMap gridMap;
    [SerializeField]
    private GridSubMap pollutionMap;
    [SerializeField]
    private Pollution pollutionPrefab;
    [SerializeField]
    private List<Pollution> pollutionObjects = new List<Pollution>();

    [SerializeField]
    private MapEffectType pollutionBlockEffect;

    //this is a helper buffer to assist with another function.  maybe scope it to that function only
    private List<Pollution> workingPollutionObjects;

    [SerializeField, Range(0, 1)]
    private float spreadBaseProbability;
    [SerializeField]
    private float spreadPeriod;
    private float spreadTimer = 0;

    GameObjectPool pollutionPool;
    
    List<Vector2Int> freePositions;

    private void Awake()
    {
        pollutionPool = new GameObjectPool(pollutionPrefab.gameObject, parentObj: gridMap.gameObject, activeByDefault: false);
        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);
        
        freePositions = new List<Vector2Int>();

        for (int x = 0; x < gridMap.width; x++)
        {
            for (int y = 0; y < gridMap.height; y++)
            {
                freePositions.Add(new Vector2Int(x, y));
            }
        }

        foreach (GridTransform gt in pollutionMap.GetAllObjects())
        {
            Pollution pToAdd = gt.GetComponent<Pollution>();
            pToAdd.PollutionManager = this;
            pollutionObjects.Add(pToAdd);
            freePositions.Remove(pToAdd.GetComponent<GridTransform>().topLeftPosMap);
        }

        PopulateInitialPollution();
        
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
        while(spreadTimer >= spreadPeriod)
        {
            spreadTimer -= spreadPeriod;
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
            foreach (MapEffectObject effectObject in effectsAtCell)
            {
                if (effectObject.EffectType == pollutionBlockEffect)
                {
                    addPollution = false;
                    break;
                }
            }
        }

        if(addPollution)
        {
            GameObject newGO = pollutionPool.GetGameObject();
            newGO.SetActive(true);
            newGO.GetComponent<GridTransform>().MoveToMapCoords(cell);
            Pollution newPollution = newGO.GetComponent<Pollution>();
            pollutionObjects.Add(newPollution);
            newPollution.PollutionManager = this;
            freePositions.Remove(cell);
            newPollution.SetAmount(newPollution.MaxAmount);
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
        freePositions.Add(pollutionPosition);
        pollutionObjects.Remove(pollution);
        pollutionPool.RecycleObject(pollution.gameObject);
    }
   
}
