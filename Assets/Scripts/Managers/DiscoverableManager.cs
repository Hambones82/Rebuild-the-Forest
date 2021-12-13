using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DiscoverableManager : MonoBehaviour
{
    public delegate void DiscoverableChangeDelegate(int x, int y, bool b);
    public event DiscoverableChangeDelegate OnDiscoverableChange;

    private bool[,] _discovered; //don't set this directly -- doing so fails to call callbacks
    private int width, height;

    public void SetDiscovered(int x, int y, bool b)
    {
        if(x < 0 || y < 0 || x >= width || y >= height)
        {
            throw new ArgumentOutOfRangeException("Trying to set discovered pollution value out of bounds of map");
        }
        else
        {
            _discovered[x, y] = b;
            OnDiscoverableChange?.Invoke(x, y, b);
        }
    }

    

    private void Awake()
    {
        width = GridMap.Current.width;
        height = GridMap.Current.height;
        _discovered = new bool[width, height];
        PollutionManager.Instance.OnInitComplete += InitializeDiscovered;
    }

    private void InitializeDiscovered()
    {
        GridSubMap pollutionGridMap = GridMap.Current.GetMapOfType(MapLayer.pollution);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetDiscovered(x,y, pollutionGridMap.GetObjectsAtCell(new Vector2Int(x,y)).Count == 0);
            }
        }
    }
}
