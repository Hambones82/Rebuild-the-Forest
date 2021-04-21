using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Assertions;


public class GridSubMap 
{ 
    public event Action mapChangeEvent;

    public int width;
    public int height;

    public MapLayer mapLayer; 

    //...?
    private GridMapCell[,] gridMapCells; //an array of gridmapcells.  

    public RectInt GetRect
    {
        get
        {
            return new RectInt(0, 0, width, height);
        }

    }

    private List<GridTransform> mapAbleObjects; //list of all grid transforms in the layer?

    //constructor
    public GridSubMap(int width, int height, MapLayer mLayer)
    {
        mapLayer = mLayer;
        this.width = width;
        this.height = height;
        gridMapCells = new GridMapCell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                gridMapCells[i, j] = new GridMapCell(i, j);
            }
        }
        mapAbleObjects = new List<GridTransform>();
        
    }

    //important functions -- get, set, etc.
    //maybe set to false if object not added for whatever reason (e.g., outside of bounds?)
    public bool AddMapObject(GridTransform mapObject) //remove = false??? wtf???
    {
        RectInt mapObjectRect = mapObject.GetRect().ClipRect(this.GetRect); //implicitly bounds checking
        
        if(mapObjectRect.IsZero())
        {
            return false;
        }
        else
        {
            for (int i = mapObjectRect.xMin; i < mapObjectRect.xMax; i++)
            {
                for (int j = mapObjectRect.yMin; j < mapObjectRect.yMax; j++)
                {
                    Assert.IsTrue(gridMapCells != null);
                    Assert.IsTrue(gridMapCells[i, j] != null);
                    Assert.IsTrue(gridMapCells[i, j].mapAbleObjects != null);
                    Assert.IsTrue(mapObject != null);
                    gridMapCells[i, j].mapAbleObjects.Add(mapObject);
                    //Debug.Log($"grid sub map is adding object at layer {mapLayer.ToString()}, cell {i}, {j}, count: {gridMapCells[i, j].mapAbleObjects.Count}");

                }
            }
            
            mapAbleObjects.Add(mapObject);
            //Debug.Log($"mapableobjects is adding object at layer {mapLayer.ToString()}, count: {mapAbleObjects.Count}");
            OnMapChangeEvent();
        }
        return true;
       
    }

    public bool RemoveMapObject(GridTransform mapObject)
    {
        RectInt mapObjectRect = mapObject.GetRect().ClipRect(this.GetRect); //implicitly bounds checking

        if (mapObjectRect.IsZero())
        {
            return false;
        }
        else
        {
            for (int i = mapObjectRect.xMin; i < mapObjectRect.xMax; i++)
            {
                for (int j = mapObjectRect.yMin; j < mapObjectRect.yMax; j++)
                {
                    Assert.IsTrue(gridMapCells != null);
                    Assert.IsTrue(gridMapCells[i, j] != null);
                    Assert.IsTrue(gridMapCells[i, j].mapAbleObjects != null);
                    Assert.IsTrue(mapObject != null);
                    
                    if (gridMapCells[i, j].mapAbleObjects.Contains(mapObject))
                    {
                        gridMapCells[i, j].mapAbleObjects.Remove(mapObject);
                        //Debug.Log($"after removing object at cell {i}, {j}, layer {mapLayer.ToString()}, cell contains {gridMapCells[i,j].mapAbleObjects.Count} objects" );
                    }
                    
                }
            }
            
            if (mapAbleObjects.Contains(mapObject))
            {
                mapAbleObjects.Remove(mapObject);
                //Debug.Log($"after removing object, layer {mapLayer.ToString()}, submap includes {mapAbleObjects.Count} objects");
            }
            
            OnMapChangeEvent();
        }

        return true;
    }

    public bool OutOfMapBounds(Vector2Int cellToCheck)
    {
        if(!GetRect.Contains(cellToCheck))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsCellOccupied(Vector2Int cellToCheck)
    {
        if(OutOfMapBounds(cellToCheck))
        {
            return false;
        }
        if(gridMapCells[cellToCheck.x, cellToCheck.y].Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<GridTransform> GetAllObjects()
    {
        return new List<GridTransform>(mapAbleObjects);
    }

    public List<GridTransform> GetObjectsAtCell(Vector2Int cellPos)
    {
        if(OutOfMapBounds(cellPos))
        {
            return null;
        }
        return gridMapCells[cellPos.x, cellPos.y].mapAbleObjects;
    }

    public bool ComponentTypeExistsAtCell<T>(Vector2Int cellPos)
    {
        if (OutOfMapBounds(cellPos))
            return false;
        List<GridTransform> gridTransforms = GetObjectsAtCell(cellPos);
        foreach(GridTransform gt in gridTransforms)
        {
            if (gt.GetComponent<T>() != null) return true;
        }
        return false;
    }

    public Color GetMinimapColor(Vector2Int cellPos)
    {
        if(OutOfMapBounds(cellPos))
        {
            return Color.white; //DEFAULT COLOR?
        }
        GridMapCell gmc = gridMapCells[cellPos.x, cellPos.y];
        if (gmc.mapAbleObjects.Count != 0)
        {
            int mObjIndex = gmc.mapAbleObjects[0].GetMiniMapPriority();
            Color retColor = gmc.mapAbleObjects[0].GetMinimapColor();
            if(gmc.mapAbleObjects.Count > 1)
            {
                for(int i = 1; i < gridMapCells[cellPos.x, cellPos.y].mapAbleObjects.Count; i++)
                {
                    if(gmc.mapAbleObjects[i].GetMiniMapPriority() > mObjIndex)
                    {
                        mObjIndex = gmc.mapAbleObjects[i].GetMiniMapPriority();
                        retColor = gmc.mapAbleObjects[i].GetMinimapColor();
                    }
                }
            }


            return retColor; //don't like using mapableobjects[0]... need something better.
        }
        else
        {
            return Color.white;
        }
    }

    public Vector2Int GetSize()
    {
        return new Vector2Int(width, height);
    }

    private void OnMapChangeEvent()
    {
        if (mapChangeEvent != null)
            mapChangeEvent();
    }

}
