using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RootManager : MonoBehaviour
{
    private static RootManager _instance;
    public static RootManager Instance { get => _instance; }

    private List<Root> _roots = new List<Root>();

    [SerializeField]
    private Root rootPrefab;

    int width;
    int height;

    private bool[,] rootPassableMap;
    private PathFinder rootPathfinder;
    
    [SerializeField]
    private Sprite[] sprites;

    private void Awake()
    {
        width = GridMap.Current.width;
        height = GridMap.Current.height;
        rootPassableMap = new bool[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                rootPassableMap[x, y] = true;
            }
        }
        rootPathfinder = new PathFinder(GridMap.Current.width, GridMap.Current.height, rootPassableMap, PathFinder.NeighborType.fourWay);
        if (_instance != null) throw new InvalidOperationException("can't have two root managers");
        else _instance = this;
    }

    public Root SpawnRoot(Vector2Int position)
    {
        if (!GridMap.Current.IsWithinBounds(position)) return null;
        if (GridMap.Current.IsCellOccupied(position, MapLayer.roots)) return null;
        else
        {
            Vector3 worldPos = rootPrefab.GetComponent<GridTransform>().TopLeftMapToWorldCenter(position);
            Root newRoot = Instantiate(rootPrefab, worldPos, Quaternion.identity, GridMap.Current.GetComponent<Transform>());
            SetConnectivity(newRoot, ConnectivityIfPlaced(position));
            _roots.Add(newRoot);
            AddToNeighborConnectivity(new Vector2Int(position.x, position.y + 1), Direction.south);
            AddToNeighborConnectivity(new Vector2Int(position.x, position.y - 1), Direction.north);
            AddToNeighborConnectivity(new Vector2Int(position.x + 1, position.y), Direction.west);
            AddToNeighborConnectivity(new Vector2Int(position.x - 1, position.y), Direction.east);
            return newRoot;
        }
    }

    private Direction ConnectivityIfPlaced(Vector2Int position)
    {
        Vector2Int north = new Vector2Int(position.x, position.y + 1);
        Vector2Int south = new Vector2Int(position.x, position.y - 1);
        Vector2Int east = new Vector2Int(position.x + 1, position.y);
        Vector2Int west = new Vector2Int(position.x - 1, position.y);
        Direction[] positionsOccupied = new Direction[4]
        {
                GridMap.Current.IsCellOccupied(north) ? Direction.north : Direction.none,
                GridMap.Current.IsCellOccupied(south) ? Direction.south : Direction.none,
                GridMap.Current.IsCellOccupied(east) ? Direction.east : Direction.none,
                GridMap.Current.IsCellOccupied(west) ? Direction.west : Direction.none
        };
        return positionsOccupied[0] | positionsOccupied[1] | positionsOccupied[2] | positionsOccupied[3];
    }

    private void SetConnectivity(Root root, Direction connectivity)
    {
        int directionCombination = (int)connectivity;
        root.GetComponent<SpriteRenderer>().sprite = sprites[directionCombination];
        root.connectivity = (Direction)directionCombination;
    }

    private void AddToNeighborConnectivity(Vector2Int neighborPos, Direction connectivityToAdd)
    {
        Root root = GridMap.Current.GetObjectAtCell<Root>(neighborPos, MapLayer.roots);
        if (root == null) return;
        SetConnectivity(root, root.connectivity | connectivityToAdd);
    }
    
    public void DeleteRoot(Root root)
    {
        _roots.Remove(root);
        Destroy(root.gameObject);
    }

    public bool GetRootPath(Vector2Int start, Vector2Int end, out List<Vector2Int> path)
    {
        return rootPathfinder.GetPath(start, end, out path);
    }
}
