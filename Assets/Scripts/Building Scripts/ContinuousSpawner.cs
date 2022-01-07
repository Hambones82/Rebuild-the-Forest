using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousSpawner : MonoBehaviour
{
    [SerializeField]
    BuildingComponentHarvestable spawnee;
    Building spawneeBuilding;

    [SerializeField]
    private bool _spawn;

    [SerializeField]
    private float time;
    [SerializeField]
    private float period;

    private void Awake()
    {
        spawneeBuilding = spawnee.GetComponent<Building>();
    }

    public void StartSpawning()
    {
        _spawn = true;
    }

    public void StopSpawning()
    {
        _spawn = false;
    }

    private void Update()
    {
        if(_spawn)
        {
            time += Time.deltaTime;
            if(time >= period)
            {
                List<Vector2Int> adjacentTiles = GetComponent<GridTransform>().GetAdjacentTiles();
                
                for(int i = adjacentTiles.Count-1; i >=0 ; i--)
                {
                    if(GridMap.Current.IsCellOccupied(adjacentTiles[i], MapLayer.buildings))
                    {
                        adjacentTiles.RemoveAt(i);
                    }
                }
                if(adjacentTiles.Count != 0)
                {
                    Vector2Int selectedTile = adjacentTiles[Random.Range(0, adjacentTiles.Count)];
                    BuildingManager.Instance.SpawnBuildingAt(spawneeBuilding, selectedTile);
                }
                time = 0;
            }
        }
    }
}
