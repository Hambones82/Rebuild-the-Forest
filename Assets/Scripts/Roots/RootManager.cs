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
        else
        {
            Vector3 worldPos = rootPrefab.GetComponent<GridTransform>().TopLeftMapToWorldCenter(position);
            Root newRoot = Instantiate(rootPrefab, worldPos, Quaternion.identity, GridMap.Current.GetComponent<Transform>());
            _roots.Add(newRoot);
            return newRoot;
        }
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
