using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentContinuousSpawner : MonoBehaviour
{
    [SerializeField]
    BuildingComponentHarvestable spawnee;
    Building spawneeBuilding;
    
    [SerializeField]
    private float period;

    private Timer spawnTimer;

    private void Awake()
    {
        spawneeBuilding = spawnee.GetComponent<Building>();
        spawnTimer = new Timer(period);
        spawnTimer.Enabled = true;
        spawnTimer.AddEvent(Spawn);
    }

    public void StartSpawning()
    {        
        spawnTimer.Enabled = true;
    }

    public void StopSpawning()
    {
        spawnTimer.Enabled = false;
    }
    
    
    public void Spawn()
    {
        List<Vector2Int> adjacentTiles = GetComponent<GridTransform>().GetAdjacentTiles();
        
        for(int i = adjacentTiles.Count-1; i >=0 ; i--)
        {
            if (GridMap.Current.IsCellOccupied(adjacentTiles[i], MapLayer.buildings))
            {
                adjacentTiles.RemoveAt(i);
            }
        }
        if(adjacentTiles.Count != 0)
        {
            Vector2Int selectedTile = adjacentTiles[Random.Range(0, adjacentTiles.Count)];
            BuildingManager.Instance.SpawnBuildingAt(spawneeBuilding, selectedTile);
        }
    }
    
    private void Update()
    {
        spawnTimer.UpdateTimer();
    }
    
}
