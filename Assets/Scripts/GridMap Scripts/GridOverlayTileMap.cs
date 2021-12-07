using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

//the purpose of this is to display the grid
//it uses the built in tile map function to do so

public class GridOverlayTileMap : MonoBehaviour {

    public TileBase overlayCell;
    private GridMap gridMap;
    private Tilemap tilemap;
    
	void Start () {
        gridMap = GetComponentInParent<GridMap>();
        tilemap = GetComponent<Tilemap>();
        RectInt gridMapRect = gridMap.GetGridRect();
        tilemap.size = new Vector3Int(gridMapRect.width, gridMapRect.height, 0);
        Vector2Int mapOrigin = gridMap.MapToGrid(new Vector2Int(0, 0));
        tilemap.origin = new Vector3Int(mapOrigin.x, mapOrigin.y, 0);
        tilemap.ResizeBounds();
        tilemap.FloodFill(Vector3Int.zero, overlayCell);
	}
	
}
