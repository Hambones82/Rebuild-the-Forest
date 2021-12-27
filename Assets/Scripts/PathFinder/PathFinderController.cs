using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-4)]
public class PathFinderController : MonoBehaviour
{
    private void Awake()
    {
        int width = GridMap.Current.width;
        int height = GridMap.Current.height;
        //initialize pathfinder
        bool[,] passableMap = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                passableMap[x, y] = ((TerrainTile)(GridMap.Current.GetTileAt(typeof(TerrainTile), new Vector2Int(x, y)))).Buildable;
            }
        }

        PathFinder.Initialize(width, height, passableMap);
    }
}
