using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//maybe make a dervied type, e.g., level1controller...  level2controller... defines some additional addpollution stuff.
// the other option is to make a flat class... have an interface or something.  seems harder...  not sure.
[System.Serializable]
public abstract class BasicPollutionController : PollutionTypeController
{   
    //this is a helper buffer to assist with another function.  maybe scope it to that function only
    private List<Pollution> workingPollutionObjects;

    [SerializeField, Range(0, 1)]
    private float spreadBaseProbability;
    
    private List<Vector2Int> freePositions;
    
    //can probably put this into base...  
    public override void Initialize(GridMap inGridMap, PollutionManager inPollutionManager)
    {
        base.Initialize(inGridMap, inPollutionManager);
        
        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);

        freePositions = new List<Vector2Int>();

        bool initialMapHasPollution = false;

        foreach (GridTransform gt in pollutionMap.GetAllObjects())
        {
            initialMapHasPollution = true;
            Pollution pToAdd = gt.GetComponent<Pollution>();
            pToAdd.PollutionManager = pollutionManager;
            pollutionObjects.Add(pToAdd);
            UpdateFreePositionsForAddition(pToAdd.GetComponent<GridTransform>().topLeftPosMap);
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

    protected abstract bool IsBlocked(Vector2Int cell);
    
    //this part -- check for the ...  priority.  don't add if... higher (lower?) priority pollution already exists at cell ... but maybe we just want to 
    //handle this w free positions?
    private Pollution AddPollution(Vector2Int cell)
    {
        bool addPollution = !IsBlocked(cell); //something other than isblocked?
        if (addPollution) //all of this can be put into base
        {
            GameObject newGO = pollutionPool.GetGameObject();
            newGO.GetComponent<GridTransform>().MoveToMapCoords(cell);
            newGO.SetActive(true);
            Pollution newPollution = newGO.GetComponent<Pollution>();
            pollutionObjects.Add(newPollution);
            newPollution.PollutionManager = this.pollutionManager;
            newPollution.SetAmount(newPollution.MaxAmount);
            UpdateFreePositionsForAddition(cell); //make this virtual?
            return newPollution;
        }
        else
        {
            return null;
        }
    }

    private void UpdateFreePositionsForAddition(Vector2Int cell)
    {
        if (freePositions.Contains(cell))
        {
            RemoveFreePosition(cell);
        }

        List<Vector2Int> positions = cell.GetNeighbors();

        foreach (Vector2Int position in positions)
        {
            if (!freePositions.Contains(position) && gridMap.IsWithinBounds(position) && !pollutionMap.IsCellOccupied(position))//don't add if priority is wrong
            {
                AddFreePosition(position);
            }
        }
    }

    private void UpdateFreePositionsForRemoval(Vector2Int cell)
    {
        List<Vector2Int> positions = cell.GetNeighbors();
        bool neighborsPollution = false;
        foreach (Vector2Int position in positions)
        {
            if (pollutionMap.IsCellOccupied(position))
            {
                neighborsPollution = true;
            }
            List<Vector2Int> subPositions = position.GetNeighbors();
            bool subNeighborsPollution = false;
            if (freePositions.Contains(position))
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
        DebugTilemap.Instance.RemoveTile(cell);
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

    //could be in base
    public override void RemovePollution(Pollution pollution, Vector2Int pollutionPosition)
    {
        base.RemovePollution(pollution, pollutionPosition);
        UpdateFreePositionsForRemoval(pollutionPosition);
    }
}
