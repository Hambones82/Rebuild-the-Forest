using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//fix the... dict thing... make it more generic so we can use it here...

public class BuildingManager : MonoBehaviour {

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
    }

    [SerializeField]
    private List<BuildingType> availableBuildingTypes;

    [SerializeField]
    public GridMap gridMap;

    [SerializeField]
    public Vector2Int defaultSpawnCoords;

    private List<Building> _buildings = new List<Building>();

    public Building SpawnBuilding(Building building)
    {

        return SpawnBuildingAt(building, defaultSpawnCoords);
    }
    
    public Building SpawnBuildingAt(Building buildingPrefab, Vector2Int MapCoords)
    {
        Building building = Instantiate(buildingPrefab, gridMap.GetComponent<Transform>());
        building.GetComponent<GridTransform>().MoveToMapCoords(MapCoords);
        _buildings.Add(building);
        return building;
    }

    //worldCoords is supposed to be the center
    public Building SpawnBuildingAt(Building buildingPrefab, Vector3 worldCoords)
    {
        Building building = SpawnBuilding(buildingPrefab);
        building.GetComponent<GridTransform>().MoveToWorldCoords(worldCoords);
        return building;
    }
    
    public Building SpawnBuilding(BuildingType buildingType)
    {
        return SpawnBuilding(buildingType.BuildingPrefab);
    }

    public Building SpawnBuildingAt(BuildingType buildingType, Vector2Int MapCoords)
    {
        return SpawnBuildingAt(buildingType.BuildingPrefab, MapCoords);
    }

    //checks that:
    //(a) terrain is buildable; and
    //(b) there is no overlapping building
    public bool CanPlaceBuildingAt(Building building, Vector2Int mapCoords)
    {
        GridTransform buildingGT = building.GetComponent<GridTransform>();
        for(int x = mapCoords.x; x < mapCoords.x + buildingGT.Width; x++)
        {
            for(int y = mapCoords.y; y > mapCoords.y - buildingGT.Height; y--)
            {
                Vector2Int coords = new Vector2Int(x, y);
                if (gridMap.IsCellOccupied(coords, MapLayer.buildings))
                {
                    Debug.Log("Can't place building - blocked by map.");
                    return false;
                }
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
        return new List<BuildingType>(availableBuildingTypes);
    }
}
