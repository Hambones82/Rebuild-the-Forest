using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicPollutionController : PollutionTypeController
{
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
    
    private List<Vector2Int> freePositions;
    
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

    private Pollution AddPollution(Vector2Int cell)
    {
        bool addPollution = true;
        List<MapEffectObject> effectsAtCell = MapEffectsManager.Instance.GetEffectsAtCell(cell);
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

        if (addPollution)
        {
            GameObject newGO = pollutionPool.GetGameObject();
            newGO.SetActive(true);
            newGO.GetComponent<GridTransform>().MoveToMapCoords(cell);
            Pollution newPollution = newGO.GetComponent<Pollution>();
            pollutionObjects.Add(newPollution);
            newPollution.PollutionManager = this.pollutionManager;
            newPollution.SetAmount(newPollution.MaxAmount);
            UpdateFreePositionsForAddition(cell);
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
            if (!freePositions.Contains(position) && gridMap.IsWithinBounds(position) && !pollutionMap.IsCellOccupied(position))
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
        if (neighborsPollution)
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

    private void AddFreePosition(Vector2Int cell)
    {
        freePositions.Add(cell);
        DebugTilemap.Instance.AddTile(cell);
    }

    private void RemoveFreePosition(Vector2Int cell)
    {
        freePositions.Remove(cell);
        DebugTilemap.Instance.RemoveTile(cell);
    }

    protected override void UpdatePollutionState()
    {
        if (freePositions.Count > 0)
        {
            int position = UnityEngine.Random.Range(0, freePositions.Count);
            AddPollution(freePositions[position]);
        }
    }

    public override void RemovePollution(Pollution pollution, Vector2Int pollutionPosition)
    {
        base.RemovePollution(pollution, pollutionPosition);
        UpdateFreePositionsForRemoval(pollutionPosition);
    }
}
