using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


[DefaultExecutionOrder(-9)]
public class GridTransform : MonoBehaviour, IGridMapable
{
    public delegate void ChangeMapPosDelegate(Vector2Int amountMoved);
    public event ChangeMapPosDelegate OnChangeMapPos;

    [SerializeField]
    private bool snapToGrid = true;

    public MapLayer mapLayer; //higher means higher priority, just like order in layer on sprite renderer
    public Color MiniMapColor;


    public bool useParentGrid = true;//must mean -- upon awake, assign parent as the grid...  shrug...

    //parent grid
    public GridMap gridMap;

    //GENERAL VARIABLES
    [SerializeField]
    private int width;
    public int Width
    {
        get { return width; }
    }
    [SerializeField]
    private int height;
    public int Height
    {
        get { return height; }
    }

    //these are correct
    [SerializeField]
    private float worldHeight;
    [SerializeField]
    private float worldWidth;

    //MAP COORDINATES
    public Vector2Int topLeftPosMap = new Vector2Int(0, 0); //position in coordinates of the map, not the grid coordaintes, which can be negative

    //WORLD COORDINATES
    private Vector3 topLeftWorldPos = new Vector3(0, 0, 0);

    //INTERNAL VALUE FOR CALCULATIONS
    //[SerializeField]
    private Vector3 topLeftToCenterDistance = new Vector3(0, 0, 0);

    //WHETHER THIS IS REGISTERED TO THE GRIDMAP
    public bool registeredInMap = true;

    public void DisableGridTransform()//?
    {
        if (registeredInMap)
        {
            DeRegisterFromMap();
        }
    }

    public void EnableGridTransform()
    {
        if (registeredInMap)
        {
            RegisterToMap();
        }
    }

    public void SetSize(Vector2Int size)
    {
        width = size.x;
        height = size.y;
        CalculateTopLeftToCenterDistance();
        worldWidth = gridMap.mapCellToCellDistance.x * width;
        worldHeight = gridMap.mapCellToCellDistance.y * height;
    }

    public void SetSize(GridTransform gtToCopy)
    {
        SetSize(new Vector2Int(gtToCopy.width, gtToCopy.height));
    }

    //the move to stuff... 
    public void MoveToMapCoords(Vector2Int mapCoords)
    {

        if (mapCoords == topLeftPosMap)
        {
            return;
        }
        MoveToRawWorldCoords(TopLeftMapToWorldCenter(mapCoords));

    }

    public void MoveToWorldCoords(Vector3 worldCoords)
    {

        Vector3 worldDestination;
        //snapping here is unnecessary, since we do it in "align in map" -- though that should probably be broken up?
        if (snapToGrid)
        {
            Vector2Int newMapCoords = WorldCenterToMapTopLeft(worldCoords);
            worldDestination = TopLeftMapToWorldCenter(newMapCoords);
        }

        else
        {
            worldDestination = worldCoords;
        }

        MoveToRawWorldCoords(worldDestination);
    }

    private void MoveToRawWorldCoords(Vector3 worldCoords)
    {
        Assert.IsNotNull(gridMap);
        //save current map coords (possibly entire grid transform?)
        Vector2Int oldMapCoords = topLeftPosMap;
        if (registeredInMap && isActiveAndEnabled)
        {
            DeRegisterFromMap();
        }
        transform.position = worldCoords;
        if (snapToGrid)
        {
            SnapAndConstrainToBounds();
        }
        else
        {
            ConstrainToBoundsWithoutSnap();
        }
        if (registeredInMap && isActiveAndEnabled)
        {
            RegisterToMap();
        }
        //why don't i do onleavemappos
        //and then onarrivemappos -- that's a pretty good way to do it...  problem is...  there's no way to calculate the map pos from a world pos without
        //changing those things...  
        //OK -- I have an idea. use distance moved.  call if distance moved is not 0.  this is a good idea becasuse the distance moved can be used to calculate
        //whatever you want.  
        Vector2Int amountMoved = topLeftPosMap - oldMapCoords;
        if (amountMoved != Vector2Int.zero)
        {
            OnChangeMapPos?.Invoke(amountMoved);
            //Debug.Log("transform is moving cells");
        }
    }

    public List<Vector2Int> GetAdjacentTiles()
    {
        List<Vector2Int> retVal = new List<Vector2Int>();
        //two rows plus two truncated columns
        for (int i = topLeftPosMap.x - 1; i <= topLeftPosMap.x + width; i++)
        {
            Vector2Int topCell = new Vector2Int(i, topLeftPosMap.y + 1);
            Vector2Int bottomCell = new Vector2Int(i, topLeftPosMap.y - height);
            if (gridMap.IsWithinBounds(topCell))
            {
                retVal.Add(topCell);
            }
            if (gridMap.IsWithinBounds(bottomCell))
            {
                retVal.Add(bottomCell);
            }
        }
        for (int i = topLeftPosMap.y - height + 1; i <= topLeftPosMap.y; i++)
        {
            Vector2Int leftCell = new Vector2Int(topLeftPosMap.x - 1, i);
            Vector2Int rightCell = new Vector2Int(topLeftPosMap.x + width, i);
            if (gridMap.IsWithinBounds(leftCell))
            {
                retVal.Add(leftCell);
            }
            if (gridMap.IsWithinBounds(rightCell))
            {
                retVal.Add(rightCell);
            }
        }

        return retVal;
    }

    private void OnDestroy()
    {
        DisableGridTransform();
    }

    private void OnEnable()
    {
        EnableGridTransform();
    }

    private void OnDisable()
    {
        DisableGridTransform();
    }

    private void Awake()
    {
        //set sprite renderer layer based on map layer above
        GetComponent<SpriteRenderer>().sortingOrder = MLayerToSortingOrder.defs[mapLayer];
        gridMap = gameObject.GetComponentInParent(typeof(GridMap)) as GridMap;
        CalculateTopLeftToCenterDistance();
        if (snapToGrid)
        {
            SnapAndConstrainToBounds();//is this snapping functionality?
        }
        else
        {
            ConstrainToBoundsWithoutSnap();
        }

        worldWidth = gridMap.mapCellToCellDistance.x * width;
        worldHeight = gridMap.mapCellToCellDistance.y * height;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void RegisterToMap()
    {
        gridMap.RegisterMapObject(this);
    }

    private void DeRegisterFromMap()
    {
        if (gridMap == null) Debug.Log("gridmap is null");
        gridMap.DeRegisterMapObject(this);//should be deregistermapobject

    }

    //what is this function even supposed to do?  snaps to grid and also constrains the object to the bounds of the grid.
    //problem is, it does a lot of weird things...
    //so we need to fix this...
    private void SnapAndConstrainToBounds()
    {
        SnapToGrid();
        ConstrainToMapSnapped(); //yet another component?
    }

    //public Rect mapExtents;

    //set world pos, constrain, snap, set map pos
    private void ConstrainToBoundsWithoutSnap()
    {
        //Vector3 map
        //need gridmap minxworld, minyworld, maxxworld, maxyworld
        Rect mapExtents = gridMap.GetCellExtentsWorldRect();
        bool leftOutside = false;
        bool rightOutside = false;
        bool topOutside = false;
        bool bottomOutside = false;
        Vector3 position = transform.position;
        Vector3 boundedPosition = position;
        //use of toplefttocenterdistance isn't correct.  it's in map units -- no good.
        float wD2 = worldWidth / 2;
        float hD2 = worldHeight / 2;
        float minX = position.x - wD2;
        float maxX = position.x + wD2;
        float minY = position.y - hD2;
        float maxY = position.y + hD2;
        if (minX < mapExtents.xMin)
        {
            leftOutside = true;
        }
        if (maxX > mapExtents.xMax)
        {
            rightOutside = true;
        }
        if (minY < mapExtents.yMin)
        {
            bottomOutside = true;
        }
        if (maxY > mapExtents.yMax)
        {
            topOutside = true;
        }
        if (leftOutside)
        {
            boundedPosition.x = mapExtents.xMin + wD2;
        }
        if (rightOutside)
        {
            boundedPosition.x = mapExtents.xMax - wD2;
        }
        if (topOutside)
        {
            boundedPosition.y = mapExtents.yMax - hD2;
        }
        if (bottomOutside)
        {
            boundedPosition.y = mapExtents.yMin + hD2;
        }
        transform.position = boundedPosition;
        topLeftPosMap = WorldCenterToMapTopLeft(transform.position);
    }

    //given a position in world coordinates, that is intended to be at the center of the grid transform, return the coordinates of the top left cell in map coordinates
    private Vector2Int WorldCenterToMapTopLeft(Vector3 WorldCenterPos)
    {
        Assert.IsNotNull(gridMap);

        Vector3 topLeftWorld = new Vector3(0, 0, 0);
        topLeftWorld.x = WorldCenterPos.x - topLeftToCenterDistance.x;
        topLeftWorld.y = WorldCenterPos.y + topLeftToCenterDistance.y;
        return gridMap.WorldToMap(topLeftWorld);

    }

    private Vector3 TopLeftMapToWorldCenter(Vector2Int TopLeftMap)
    {
        Vector3 worldTopLeftCoords = gridMap.MapToWorld(TopLeftMap);
        Vector3 worldCenterCoords = new Vector3(worldTopLeftCoords.x + topLeftToCenterDistance.x, worldTopLeftCoords.y - topLeftToCenterDistance.y, 0);
        return worldCenterCoords;
    }

    private void CalculateTopLeftToCenterDistance()
    {
        float adjustedWidth = (width - 1) / 2.0f;
        float adjustedHeight = (height - 1) / 2.0f;
        Vector3 cellDistance = gridMap.mapCellToCellDistance;
        topLeftToCenterDistance.x = adjustedWidth * cellDistance.x;
        topLeftToCenterDistance.y = adjustedHeight * cellDistance.y;
    }

    //this should be called when the transform is set, but I don't think this is useful when you set the cell position directly
    private void SnapToGrid()
    {
        Vector3 GOPosition = transform.position; //comes back as center position
        topLeftWorldPos.x = GOPosition.x - topLeftToCenterDistance.x;
        topLeftWorldPos.y = GOPosition.y + topLeftToCenterDistance.y;
        Vector3 snappedTopLeftWorld = gridMap.SnapWorldPos(topLeftWorldPos);
        transform.position = new Vector3(snappedTopLeftWorld.x + topLeftToCenterDistance.x, snappedTopLeftWorld.y - topLeftToCenterDistance.y, 0);
        topLeftPosMap = gridMap.WorldToMap(snappedTopLeftWorld);
    }

    private void ConstrainToMapSnapped()
    {
        bool leftOutside = false;
        bool rightOutside = false;
        bool topOutside = false;
        bool bottomOutside = false;

        if (topLeftPosMap.x < 0) leftOutside = true;
        if (topLeftPosMap.y > gridMap.height - 1) topOutside = true;
        if (topLeftPosMap.y - height + 1 < 0) bottomOutside = true;
        if (topLeftPosMap.x + width > gridMap.width) rightOutside = true;

        if (rightOutside)
        {
            topLeftPosMap.x = gridMap.width - width;
        }
        if (bottomOutside)
        {
            topLeftPosMap.y = height - 1;
        }
        if (leftOutside)
        {
            topLeftPosMap.x = 0;
        }
        if (topOutside)
        {
            topLeftPosMap.y = gridMap.height - 1;
        }

        Vector3 snappedTopLeftWorld = gridMap.MapToWorld(topLeftPosMap);
        transform.position = new Vector3(snappedTopLeftWorld.x + topLeftToCenterDistance.x, snappedTopLeftWorld.y - topLeftToCenterDistance.y, 0);
    }

    //i think this returns a rect of the transform's grid coords
    public RectInt GetRect()
    {
        return new RectInt(new Vector2Int(topLeftPosMap.x, topLeftPosMap.y - (height - 1)), new Vector2Int(width, height));
    }

    public Color GetMinimapColor()
    {
        return MiniMapColor;
    }

    public int GetMiniMapPriority()
    {
        return MLayerToSortingOrder.defs[mapLayer];//???  i guess ok?
    }

    //works on an already-placed gridTransform
    public bool AtLeastOneCellIsOccupiedBy(MapLayer mapLayer)
    {
        Vector2Int mapCoords = topLeftPosMap;
        foreach (Vector2Int coords in GetRect().allPositionsWithin)
        {
            if (GridMap.Current.IsCellOccupied(coords, mapLayer))
            {
                return true;
            }
        }
        return false;
    }

    public bool WouldBeOccupiedAtPosition(Vector2Int mapCoords, MapLayer mapLayer)
    {
        for (int x = mapCoords.x; x < mapCoords.x + Width; x++)
        {
            for (int y = mapCoords.y; y > mapCoords.y - Height; y--)
            {
                Vector2Int coords = new Vector2Int(x, y);
                if (GridMap.Current.IsCellOccupied(coords, mapLayer))
                {
                    return true;
                }
            }
        }
        return false;
    }
}