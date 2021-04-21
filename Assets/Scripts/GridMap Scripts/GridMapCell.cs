using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this is a single cell of the grid map.
public class GridMapCell
{
    public List<GridTransform> mapAbleObjects;
    
    public Vector2Int mapPosition;
    

    public int Count
    {
        get
        {
            return mapAbleObjects.Count;
        }
    }

    public GridMapCell(int x, int y)
    {
        mapAbleObjects = new List<GridTransform>();
        mapPosition = new Vector2Int(x, y);
    }
      
}
