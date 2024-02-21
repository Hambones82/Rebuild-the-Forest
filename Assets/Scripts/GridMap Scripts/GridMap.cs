using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

//want to have this use a list of maps. each map is for a different mappable monobehaviour component
[DefaultExecutionOrder(-10)] //the reason for this is we want the map to be initialized before things that rely on the map.  for example, the grid transforms rely on the map
public class GridMap : MonoBehaviour { //maybe this imapdisplayable thing is for those individual maps

    private static GridMap _current;
    public static GridMap Current { get => _current; }

    public bool showBordersInEditor;

    public Vector2Int bottomLeftWorldCell;
    public Grid grid;

    public event Action mapChangeEvent;

    //refactor the next thing as vector2int - or even rect?
    public int width;
    public int height;
    
    //one grid sub map for each map type
    //these maps are for gridtransforms
    private List<TileDataMap> tileDataMaps = new List<TileDataMap>(); 
    private List<GridSubMap> gridSubMaps;//? -- basically, looks like map layers...
    //list tilemapdata... and define tilemapdata.  it might have a type for a tiletype.

    //also do background... background just based on a tilemap...
    public TileDataMap GetTileDataMapOfType(Type type)
    {
        foreach(TileDataMap map in tileDataMaps)
        {
            if(map.IsOfType(type))
            {
                return map;
            }
        }
        return null;
    }

    public TileBase GetTileAt(Type tileType, Vector2Int coords)
    {
        if(!tileType.IsSubclassOf(typeof(TileBase)))
        {
            throw new InvalidOperationException($"type of tile to get ({tileType.ToString()}) is not a subclass of TileBase");
        }
        return GetTileDataMapOfType(tileType).GetTileAt(coords);
    }
    
    void Awake()
    {
        _current = this;

        foreach(TileDataMap tileDataMap in gameObject.transform.GetComponentsInChildren<TileDataMap>())
        {
            tileDataMaps.Add(tileDataMap);
        }
        //for each type in tilemap enums, add an entry to the list.
        //for each type in enums...
        gridSubMaps = new List<GridSubMap>();
        //ok so the problem is... we want to have a type.  i guess?
        foreach(MapLayer mapLayer in Enum.GetValues(typeof(MapLayer)))
        {
            gridSubMaps.Add(new GridSubMap(width, height, mapLayer));
        }
    }
    
    //so... the terrain could be a map layer... the problem is... we're returning a gridsubmap...  which isn't necessarily what we want.
    //i.e., gridsubmap explicitly uses gridtransform, but our terrain map is 
    private bool MapOfTypeExists(MapLayer mLayer)
    {
        Assert.IsNotNull(gridSubMaps);
        foreach(GridSubMap gridSubMap in gridSubMaps)
        {
            if (gridSubMap.mapLayer == mLayer)
            {
                return true;
            }
        }
        return false;
    }

    public GridSubMap GetMapOfType(MapLayer mLayer)
    {
        foreach(GridSubMap gridSubMap in gridSubMaps)
        {
            if (gridSubMap.mapLayer == mLayer)
            {
                return gridSubMap;
            }
        }
        return null;
    }
    //this needs to constrain to the width and height of the map itself.

    public void RegisterMapObject(GridTransform mapObject) 
    {
        if (!MapOfTypeExists(mapObject.mapLayer))
        {
            GridSubMap mapToAdd = new GridSubMap(width, height, mapObject.mapLayer);
            mapToAdd.mapLayer = mapObject.mapLayer;
            gridSubMaps.Add(mapToAdd);
            //create a new map
        }
        GridSubMap gridSubMap = GetMapOfType(mapObject.mapLayer);
        gridSubMap.AddMapObject(mapObject);
    }

    public void DeRegisterMapObject(GridTransform mapObject)
    {
        if (!MapOfTypeExists(mapObject.mapLayer))
        {
            return;
        }
        //Debug.Log("grid map is removing object");
        GridSubMap gridSubMap = GetMapOfType(mapObject.mapLayer);
        gridSubMap.RemoveMapObject(mapObject);
    }
    //pretty cool -- lets you check if grid has a mapable at a given cell - useful for player logic
    public bool IsCellOccupied(Vector2Int cellToCheck, MapLayer mapLayer) 
    {
        if (!MapOfTypeExists(mapLayer))
        {
            return false;
        }

        return (GetMapOfType(mapLayer).IsCellOccupied(cellToCheck));
    }

    public bool IsCellOccupied<T>(Vector2Int cellToCheck, MapLayer mapLayer)
    {
        return GetObjectAtCell<T>(cellToCheck, mapLayer) != null;
    }

    public bool IsCellOccupied(Vector2Int cellToCheck)
    {
        foreach(GridSubMap gsm in gridSubMaps)
        {
            if(gsm.IsCellOccupied(cellToCheck))
            {
                return true;
            }
        }
        return false;
    }

    //the above stuff needs to be implemented in grid sub map.  then we need wrappers i guess

    public bool IsWithinBounds(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height)
            return false;
        else
            return true;
    }

    public Vector2Int GetSize()
    {
        return new Vector2Int(width, height);
    }

    public RectInt GetGridRect()
    {
        return new RectInt(MapToGrid(new Vector2Int(0, 0)), GetSize()); // extents in grid coordinates
    }

    public RectInt GetMapRect() => new RectInt(new Vector2Int(0, 0), GetSize());

    public Rect GetCellCenterWorldRect()
    {
        RectInt gridExtents = GetGridRect();
        Vector3 bottomLeft = grid.CellToWorld(new Vector3Int(gridExtents.min.x, gridExtents.min.y, 0)) - grid.cellSize/2;
        Vector3 size = (grid.CellToWorld(new Vector3Int(gridExtents.max.x, gridExtents.max.y, 0)) + grid.cellSize / 2) - bottomLeft;
        Vector2 bottomLeftTrunc = new Vector2(bottomLeft.x, bottomLeft.y);
        Vector2 sizeTrunc = new Vector2(size.x, size.y);
        return new Rect(bottomLeftTrunc, sizeTrunc);
    }

    public Rect GetCellExtentsWorldRect()
    {
        RectInt gridExtents = GetGridRect();
        Vector3 bottomLeft = grid.CellToWorld(new Vector3Int(gridExtents.min.x, gridExtents.min.y, 0));
        Vector3 size = (grid.CellToWorld(new Vector3Int(gridExtents.max.x, gridExtents.max.y, 0))) - bottomLeft;
        Vector2 bottomLeftTrunc = new Vector2(bottomLeft.x, bottomLeft.y);
        Vector2 sizeTrunc = new Vector2(size.x, size.y);
        return new Rect(bottomLeftTrunc, sizeTrunc);
    }

    public Vector3 mapCellToCellDistance
    {
         get
            {
            Vector3 retVal = new Vector3(0, 0, 0);
            retVal.x = grid.cellSize.x + grid.cellGap.x;
            retVal.y = grid.cellSize.y + grid.cellGap.y;
            return retVal;
            }
    }

    public Vector3 SnapWorldPos(Vector3 posToSnap)
    {
        Vector3Int gridCellNum = grid.WorldToCell(posToSnap);
        return grid.GetCellCenterWorld(gridCellNum);
    }

    public Vector2Int MapToGrid(Vector2Int mapCoords)
    {
        Vector2Int retVal = new Vector2Int(0, 0);
        retVal.x = mapCoords.x + bottomLeftWorldCell.x;
        retVal.y = mapCoords.y + bottomLeftWorldCell.y;
        return retVal;
    }

    public Vector2Int GridToMap(Vector2Int gridCoords)
    {
        Vector2Int retVal = new Vector2Int(0, 0);
        retVal.x = gridCoords.x - bottomLeftWorldCell.x;
        retVal.y = gridCoords.y - bottomLeftWorldCell.y;
        return retVal;
    }

    public Vector2Int WorldToMap(Vector3 worldCoords)
    {
        Vector3Int gridCoords = grid.WorldToCell(worldCoords);
        return new Vector2Int(gridCoords.x - bottomLeftWorldCell.x, gridCoords.y - bottomLeftWorldCell.y);
    }

    public Vector3 MapToWorld(Vector2Int mapCoords)
    {
        Vector2Int gridCoords = MapToGrid(mapCoords);
        return grid.GetCellCenterWorld(new Vector3Int(gridCoords.x, gridCoords.y, 0));
    }

    public Vector2 MapSizeToWorldSize(Vector2Int mapSize)
    {
        Vector3 unitSize = mapCellToCellDistance;
        float worldX = mapSize.x * unitSize.x;
        float worldY = mapSize.y * unitSize.y;
        return new Vector2(worldX, worldY);
    }
    
    private void DrawMapBorders()
    {
        Vector3 cellSize = grid.cellSize / 2;
        Vector3Int topLeftWorldCell = new Vector3Int(bottomLeftWorldCell.x, bottomLeftWorldCell.y + height-1, 0);
        Vector3Int bottomRightWorldCell = new Vector3Int(bottomLeftWorldCell.x+width-1, bottomLeftWorldCell.y, 0);
        Vector3 mapTopLeftWorld = grid.GetCellCenterWorld(topLeftWorldCell);
        mapTopLeftWorld.x -= cellSize.x;
        mapTopLeftWorld.y += cellSize.y;
        Vector3 mapBottomRightWorld = grid.GetCellCenterWorld(bottomRightWorldCell);
        mapBottomRightWorld.x += cellSize.x;
        mapBottomRightWorld.y -= cellSize.y;
        
        GizmosExtensionMethods.DrawCameraParallelRectangle(mapTopLeftWorld, mapBottomRightWorld);
    }

    private void OnDrawGizmos()
    {
        if(showBordersInEditor) DrawMapBorders();
    }

    public List<GridTransform> GetObjectsAtCell(Vector2Int cellPos)
    {
        List<GridTransform> retVal = new List<GridTransform>();
        foreach(GridSubMap subMap in gridSubMaps)
        {
            List<GridTransform> rangeToAdd = subMap.GetObjectsAtCell(cellPos);
            if(rangeToAdd != null)
            {
                retVal.AddRange(rangeToAdd);
            }
        }
        return retVal;
    }

    //wow... get all objects of a particular type at a given cell.  we should do a non-generic version that gets all objects, period
    public List<GridTransform> GetObjectsAtCell(Vector2Int cellPos, MapLayer mapLayer)
    {
        if(MapOfTypeExists(mapLayer))
        {
            return GetMapOfType(mapLayer).GetObjectsAtCell(cellPos);
        }
        else
        {
            return new List<GridTransform>();
        }
    }

    public T GetObjectAtCell<T>(Vector2Int cellPos, MapLayer mapLayer) 
    {
        List<GridTransform> grids = GetObjectsAtCell(cellPos, mapLayer);
        if (grids == null) return default(T);
        foreach(GridTransform gt in grids)
        {
            T component = gt.GetComponent<T>();
            if (component != null)
                return component;
        }
        return default(T);
    }

    private GridTransform GetClosestGridTransform(List<GridTransform> gridTransforms)
    {
        if (gridTransforms == null) // 
        {
            return null;
        }
        if(gridTransforms.Count == 0)
        {
            return null;
        }
        GridTransform closestGridTransform = ((GridTransform)gridTransforms[0]);
        int closestLayer = MLayerToSortingOrder.defs[closestGridTransform.mapLayer];
        if (gridTransforms.Count > 1)
        {
            for (int i = 1; i < gridTransforms.Count; i++)
            {
                int tryLayer = MLayerToSortingOrder.defs[gridTransforms[i].mapLayer];
                if (MLayerToSortingOrder.defs[gridTransforms[i].mapLayer] > closestLayer)
                {
                    closestGridTransform = (gridTransforms[i]);
                    closestLayer = tryLayer;
                }
            }
        }
        return closestGridTransform;
        
    }

    public GridTransform GetClosestClickedObject(Vector3 mouseWorldPosition)
    {
        List<GridTransform> gridTransforms = GetObjectsAtCell(WorldToMap(mouseWorldPosition));
        return GetClosestGridTransform(gridTransforms);
    }

    //gets the closest clicked object
    //why not return IGridMapable like the rest?
    public GridTransform GetClosestClickedObject(Vector3 mouseWorldPosition, MapLayer mapLayer)
    {
        Vector2Int clickedCell = WorldToMap(mouseWorldPosition);
        List<GridTransform> gridTransforms = GetObjectsAtCell(clickedCell, mapLayer);
        return GetClosestGridTransform(gridTransforms);
        
    }
}
