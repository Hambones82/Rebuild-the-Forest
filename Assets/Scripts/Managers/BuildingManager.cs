using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix the... dict thing... make it more generic so we can use it here...

public class BuildingManager : MonoBehaviour {

    public GridMap gridMap;

    public UIManager uiManager;

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
    

    //this one is for whether you can start placement -- i.e., it's intended to show that you can even start considering placing the building
    //not sure about this one.. maybe delete...
    //placeholder???
    public bool CanPlaceBuilding(Building building)
    {
        //just a placeholder for now
        return true;
    }

    //check all building cells
    //maybe we can do gridmap.overlap or grid transform.overlap?
    public bool CanPlaceBuildingAt(Building building, Vector2Int mapCoords)
    {
        //maybe this should just be gridtransform.overlaps?
        //or possibly... a function to check if any cells of a grid transform are occupied in any particular layer...
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
                //Debug.Log(terrainTile?.ToString());
                if(!terrainTile.Buildable)
                {
                    Debug.Log("Can't place building - terrain is not buildable");
                    return false;
                }
            }
        }
        //Debug.Log("map allows building placement.");
        return true;
    }
}
