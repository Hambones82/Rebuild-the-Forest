using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[DefaultExecutionOrder(-4)]
public class DebugTilemap : MonoBehaviour
{
    //add the default alpha value
    private const float defaultAlpha = 0.8f;
    [SerializeField]
    private Tile debugTile;

    private TileDataMap tileDataMap;

    private static DebugTilemap _instance;
    public static DebugTilemap Instance
    {
        get => _instance;        
    }

    /*
     // this totally works, which is awesome...
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            float red = UnityEngine.Random.Range(0f, 1f);
            float green = UnityEngine.Random.Range(0f, 1f);
            float blue = UnityEngine.Random.Range(0f, 1f);
            AddTile(new Vector2Int(25, 25), new Color(red, green, blue, defaultAlpha));
        }
    }
    */

    private Dictionary<Color, Tile> cachedTiles = new Dictionary<Color, Tile>();

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

    public void AddTile(Vector2Int cell, Color inColor)
    {
        Color color = inColor;
        color.a = defaultAlpha;
        if(!cachedTiles.ContainsKey(color))
        {
            //write this function -- add new tile
            Tile newTile = GameObject.Instantiate(debugTile);
            newTile.color = color;
            cachedTiles.Add(color, newTile);
        }
        tileDataMap.SetTileAt(cachedTiles[color], cell);
    }

    public void RemoveTile(Vector2Int cell)
    {
        tileDataMap.SetTileAt(null, cell);
    }
}
