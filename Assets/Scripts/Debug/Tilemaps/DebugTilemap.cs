using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class DebugTilemap : MonoBehaviour
{
    [SerializeField]
    private TileBase debugTile;

    private TileDataMap tileDataMap;

    private static DebugTilemap _instance;
    public static DebugTilemap Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        tileDataMap = GetComponent<TileDataMap>();
        if(_instance != null)
        {
            throw new InvalidOperationException("can't instantiate debugtilemap twice");
        }
        _instance = this;
    }

    public void AddTile(Vector2Int cell)
    {
        tileDataMap.SetTileAt(debugTile, cell);
    }

    public void RemoveTile(Vector2Int cell)
    {
        tileDataMap.SetTileAt(null, cell);
    }
}
