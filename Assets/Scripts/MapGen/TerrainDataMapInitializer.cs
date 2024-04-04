using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileDataMap))]
public class TerrainDataMapInitializer : MonoBehaviour
{
    [SerializeField] private List<TileBase> dirtTiles;
    [SerializeField] private List<TileBase> grassTiles;
    [SerializeField] private List<TileBase> deadgrassTiles;
    [SerializeField] private GridMap gridMap;
    [SerializeField] private TileDataMap terrainMap;

    private void Awake()
    {
        gridMap = GetComponentInParent<GridMap>();
        terrainMap = GetComponent<TileDataMap>();
        Vector2Int worldSize = gridMap.GetSize();
        terrainMap.SetSize(gridMap.GetSize());
        List<TileBase> tilesToSelectFrom = new List<TileBase>(dirtTiles);
        //tilesToSelectFrom.AddRange(grassTiles);
        //tilesToSelectFrom.AddRange(deadgrassTiles);
        int numPossibleTiles = tilesToSelectFrom.Count;
        for(int x = 0; x < worldSize.x; x++)
        {
            for(int y = 0; y < worldSize.y; y++)
            {
                TileBase selectedTile = tilesToSelectFrom[UnityEngine.Random.Range(0, numPossibleTiles - 1)];
                terrainMap.SetTileAt(selectedTile, new Vector2Int(x, y));
            }
        }

    }
}
