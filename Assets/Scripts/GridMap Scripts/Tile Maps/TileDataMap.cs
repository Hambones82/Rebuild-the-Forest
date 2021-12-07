using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[DefaultExecutionOrder(-5)]
[RequireComponent(typeof(Tilemap))]
public class TileDataMap : MonoBehaviour
{
    [SerializeField]
    private GridMap _gridMap;
    public GridMap gridMap
    {
        get
        {
            if (_gridMap == null)
                _gridMap = GetComponentInParent<GridMap>();
            return _gridMap;
        }
    }

    //the normalized thing should use the grid
    [SerializeField]
    private TileDataType tileDataType;
    public TileDataType TileDataType
    {
        get => tileDataType;
    }
    private Type tileType;
    public Type TileType
    {
        get => tileType;
    }
    
    private Tilemap tileMap;
    public Tilemap TileMap
    {
        get
        {
            if(tileMap == null)
            {
                tileMap = GetComponent<Tilemap>();
            }
            return tileMap;
        }
    }

    public void SetTileAt(TileBase tile, Vector2Int tileCoords)
    {
        Vector3Int normalizedCoords = NormalizeCellPosition(tileCoords);
        tileMap.SetTile(normalizedCoords, tile);
    }

    //hopefully this returns null... otherwise we need to do a bounds check.  or possibly the error would be ok behaviour.
    public TileBase GetTileAt(Vector2Int tileCoords)
    {
        Vector3Int normalizedCoords = NormalizeCellPosition(tileCoords);
        TileBase tile = tileMap.GetTile(normalizedCoords);
        return tile;
    }

    //the coords are the grid coords of our gridmap thing...
    private Vector3Int NormalizeCellPosition(Vector2Int coords)
    {
        //use the conversion from the gridmap, not from the grid...
        Vector3 worldPos = gridMap.MapToWorld(coords);   //CellToWorld(v3coords);
        return tileMap.WorldToCell(worldPos);//??
    }

    
    public void SetSize(Vector2Int vector2Int)
    {
        Vector3Int size = new Vector3Int(vector2Int.x, vector2Int.y, 0);
        TileMap.size = size;
        TileMap.ResizeBounds();
    }

    private void Awake()
    {
        tileMap = GetComponent<Tilemap>();
        tileType = TileDataTypes.tileDataLookup[tileDataType];
        //Debug.Log(tileMap.origin);
        //validate all the tiles.  probably better would be at editor time although i guess this would be useful in addition...
        foreach (Tile tile in tileMap.GetTilesBlock(tileMap.cellBounds))
        {
            if(tile == null)
            {
                continue;
            }
            if (!(tile.GetType().IsSubclassOf(tileType)) && tile.GetType() != tileType)
            {
                throw new InvalidOperationException($"tile data map of type {tileType} can only contain tiles of that type / incorrect type is {tile.GetType()}");
            }
            else
            {
                
                //Debug.Log($"tile data map is of type {tileType}, where tile type is {tile.GetType().ToString()}");
            }
        }
    }

    public bool IsOfType(Type type) //if this thing is of the type put in...
    {
        if(!type.IsSubclassOf(typeof(Tile)))
        {
            throw new InvalidOperationException("can only inquire whether this map has type derived from Tile");
        }
        return tileType == type;
    }

    //probably want get tile data, given cell coords
    //functions for access -- write them as needed

}
