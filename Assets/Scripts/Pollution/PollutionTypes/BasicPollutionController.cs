using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe make a dervied type, e.g., level1controller...  level2controller... defines some additional addpollution stuff.
// the other option is to make a flat class... have an interface or something.  seems harder...  not sure.
[System.Serializable]
public class BasicPollutionController : PollutionTypeController
{
    [SerializeField]//a lower number means higher priority...
    private int priority;
    //this is a helper buffer to assist with another function.  maybe scope it to that function only
    private List<Pollution> workingPollutionObjects;

    [SerializeField, Range(0, 1)]
    private float spreadBaseProbability;
    
    public override void Initialize(GridMap inGridMap, PollutionManager inPollutionManager)
    {
        base.Initialize(inGridMap, inPollutionManager);

        priority = pollutionPrefab.PollutionData.Priority;

        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);

        freePositions = new List<Vector2Int>();
    }

    public override void InitializePollutionState()
    {
        bool initialMapHasPollution = false;
        //this is broken for if you have multiple pollution priorities
        foreach (GridTransform gt in pollutionMap.GetAllObjects())
        {
            Pollution pToAdd = gt.GetComponent<Pollution>();
            if(pToAdd.Priority == priority)
            {
                initialMapHasPollution = true;
                pToAdd.PollutionManager = pollutionManager;
                pollutionObjects.Add(pToAdd);
                InvokeOnPollutionAdd(pToAdd.GetComponent<GridTransform>().topLeftPosMap, pToAdd.Priority);
            }
        }

        if (!initialMapHasPollution)
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
        }
        PopulateInitialPollution();
    }


    public override void UpdateFreePositionsForAddition(Vector2Int cell, int inPriority)
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
            if(pollAtCell != null)
            {
                cellIsOccupied = pollAtCell.Priority >= priority;
            }
            if (!freePositions.Contains(position) && gridMap.IsWithinBounds(position) && !cellIsOccupied)//don't add if priority is wrong
            {
                AddFreePosition(position);
            }
        }
    }

    //inpriority is the priority of the object that is being deleted...
    public override void UpdateFreePositionsForRemoval(Vector2Int cell, int inPriority)
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
                if(GridMap.Current.GetObjectAtCell<Pollution>(position, MapLayer.pollution).Priority == priority)
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

    [SerializeField]
    private int freeZoneWidth;

    private void PopulateInitialPollution()
    {   
        for (int y = 0; y < gridMap.height; y++)
        {
            for (int x = 0; x < gridMap.width; x++)
            {
                //left half: from 0 to width - freezone / 2
                bool spawnHere = true;
                int minMiddleX = (gridMap.width - freeZoneWidth) / 2;
                int maxMiddleX = (gridMap.width + freeZoneWidth) / 2;
                int minMiddleY = (gridMap.height - freeZoneWidth) / 2;
                int maxMiddleY = (gridMap.height + freeZoneWidth) / 2;

                if (x > minMiddleX && x < maxMiddleX && y > minMiddleY && y < maxMiddleY)
                {
                    spawnHere = false;
                }
                if (spawnHere)
                {
                    AddPollution(new Vector2Int(x, y));
                }
            }
        }
        
    }

    //could be in base
    private void AddFreePosition(Vector2Int cell)
    {
        freePositions.Add(cell);
        DebugTilemap.Instance.AddTile(cell);
    }

    //could be in base
    private void RemoveFreePosition(Vector2Int cell)
    {
        freePositions.Remove(cell);
        bool freePosAtCell = false;
        foreach(PollutionTypeController controller in PollutionManager.Instance.PollutionControllers)
        {
            if (controller.FreePositions.Contains(cell)) freePosAtCell = true;
        }
        if(!freePosAtCell)
        {
            //Debug.Log("removing free pos indicator");
            DebugTilemap.Instance.RemoveTile(cell);
        }
            
    }

    //could be in base
    protected override void UpdatePollutionState()
    {
        if (freePositions.Count > 0)
        {
            int position = UnityEngine.Random.Range(0, freePositions.Count);
            AddPollution(freePositions[position]);
        }
    }
    
}
