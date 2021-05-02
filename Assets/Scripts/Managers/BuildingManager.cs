using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix the... dict thing... make it more generic so we can use it here...

public class BuildingManager : MonoBehaviour {

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
}
