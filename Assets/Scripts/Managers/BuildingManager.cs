﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//fix the... dict thing... make it more generic so we can use it here...

[DefaultExecutionOrder(-8)]
public class BuildingManager : MonoBehaviour {

    public delegate void BuildingSpawnEventHandler(Building building);

    public event BuildingSpawnEventHandler OnBuildingSpawn;
    public event BuildingSpawnEventHandler OnBuildingDelete;

    private static BuildingManager _instance;
    public static BuildingManager Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if(_instance != null)
        {
            throw new InvalidOperationException("cannot instantiate more than one BuildingManager");
        }
        _instance = this;
        _buildings = FindObjectsOfType<Building>().ToList();
    }

    [SerializeField]
    private List<BuildingType> availableBuildingTypes;

    [SerializeField]
    public GridMap gridMap;

    [SerializeField]
    public Vector2Int defaultSpawnCoords;

    [SerializeField]
    private List<Building> _buildings = new List<Building>();
    public List<Building> Buildings { get => _buildings; }
    
    public Building SpawnBuildingAt(Building buildingPrefab, Vector2Int cell)
    {
        Vector3 worldPos = buildingPrefab.GetComponent<GridTransform>().TopLeftMapToWorldCenter(cell);
        return SpawnBuildingAt(buildingPrefab, worldPos);
    }

    public Building SpawnBuildingAt(Building buildingPrefab, Vector3 worldCoords)
    {
        Building building = Instantiate(buildingPrefab, worldCoords, Quaternion.identity, gridMap.GetComponent<Transform>());
        return FinishBuildingSpawn(building);
    }
    
    private Building FinishBuildingSpawn(Building building)
    {
        _buildings.Add(building);
        OnBuildingSpawn?.Invoke(building); //this one...
        return building;
    }

    //checks that:
    //(a) terrain is buildable; and
    //(b) there is no overlapping building
    public bool CanPlaceBuildingAt(Building building, Vector2Int mapCoords) 
    {
        GridTransform buildingGT = building.GetComponent<GridTransform>();
        if(buildingGT.WouldBeOccupiedAtPosition(mapCoords, MapLayer.buildings) || buildingGT.WouldBeOccupiedAtPosition(mapCoords, MapLayer.pollution))
        {
            //Debug.Log("cannot place building on another building or on pollution");
            return false;
        }
        for(int x = mapCoords.x; x < mapCoords.x + buildingGT.Width; x++)
        {
            for(int y = mapCoords.y; y > mapCoords.y - buildingGT.Height; y--)
            {
                Vector2Int coords = new Vector2Int(x, y);
                TerrainTile terrainTile = (TerrainTile)(gridMap.GetTileAt(typeof(TerrainTile), coords));
                
                if(!terrainTile.Buildable)
                {
                    Debug.Log("Can't place building - terrain is not buildable");
                    return false;
                }
            }
        }
        return true;
    }

    public List<BuildingType> GetAvailableBuildingTypes()
    {
        List<BuildingType> retval = new List<BuildingType>();
        for(int i = 0; i < availableBuildingTypes.Count; i++)
        {
            retval.Add(availableBuildingTypes[i]);
        }
        return retval;
    }

    public void DestroyBuilding(Building buildingToDestroy)
    {
        OnBuildingDelete?.Invoke(buildingToDestroy);
        UnityEngine.Object.Destroy(buildingToDestroy.gameObject);
    }
}
