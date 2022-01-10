using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-4)]
public class PathingController : MonoBehaviour
{
    bool[,] passableMap;
    int width;
    int height;

    private static PathingController _instance;
    public static PathingController Instance { get => _instance; }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else throw new InvalidOperationException("cannot have two pathing controllers");
        width = GridMap.Current.width;
        height = GridMap.Current.height;
        //initialize pathfinder
        passableMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                passableMap[x, y] = ((TerrainTile)(GridMap.Current.GetTileAt(typeof(TerrainTile), new Vector2Int(x, y)))).Buildable;
            }
        }

        PathFinder.Initialize(width, height, passableMap);
    }

    public void UpdatePassable(Vector2Int cell, bool passable)
    {
        passableMap[cell.x, cell.y] = passable;
        PathFinder.UpdatePassable(cell, passable);
    }
       
    public bool GetPassable(Vector2Int cell)
    {
        return passableMap[cell.x, cell.y];
    }
}
