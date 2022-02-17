using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[DefaultExecutionOrder(-1)]
public class RootManager : MonoBehaviour
{
    private static RootManager _instance;
    public static RootManager Instance { get => _instance; }

    private List<Root> _roots = new List<Root>();

    private MGNCollection<RootNetworkComponent> rootNetworks;

    [SerializeField]
    private Root rootPrefab;

    int width;
    int height;

    private bool[,] rootPassableMap;//map for the roots to grow?
    private PathFinder rootPathfinder;
    private bool[,] mapOfRoots;
    private PathFinder mapOfRootsPathfinder;
    
    [SerializeField]
    private Sprite[] sprites;

    private List<RootBuildingComponent> rootBuildings;

    private void Awake()
    {
        //initialize singleton
        if (_instance != null) throw new InvalidOperationException("can't have two root managers");
        else _instance = this;

        //Initialize width/height
        width = GridMap.Current.width;
        height = GridMap.Current.height;
        //initialize pathfinding
        rootPassableMap = new bool[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                rootPassableMap[x, y] = true;
            }
        }
        //this is necessary for growing roots towards things
        rootPathfinder = new PathFinder(GridMap.Current.width, GridMap.Current.height, rootPassableMap, NeighborType.fourWay);
    }

    private void Start()
    {
        rootNetworks = new MGNCollection<RootNetworkComponent>();
        RootNetworkComponent[] rootNs = FindObjectsOfType<RootNetworkComponent>();
        foreach (RootNetworkComponent rnc in rootNs)
        {
            rootNetworks.AddNode(rnc); //also needs to do this when a new root is added, i.e., in spawnroot
            //here...  i think what's happening is that it's not updating the rnc's original network number.  need to figure this out.
            //probably need an actual is connected to...  
        }
        Root[] roots = FindObjectsOfType<Root>();
        foreach(Root root in roots)
        {
            UpdateConnectivity(root, root.GetComponent<GridTransform>().topLeftPosMap);
        }
    }

    public Root SpawnRoot(Vector2Int position)
    {
        if (!GridMap.Current.IsWithinBounds(position)) return null;
        if (GridMap.Current.IsCellOccupied(position, MapLayer.roots)) return null;
        else
        {
            Vector3 worldPos = rootPrefab.GetComponent<GridTransform>().TopLeftMapToWorldCenter(position);
            Root newRoot = Instantiate(rootPrefab, worldPos, Quaternion.identity, GridMap.Current.GetComponent<Transform>());
            _roots.Add(newRoot);
            UpdateConnectivity(newRoot, position);
            //mapOfRoots[position.x, position.y] = true;
            rootNetworks.AddNode(newRoot.GetComponent<RootNetworkComponent>());
            return newRoot;
        }
    }

    private void UpdateConnectivity(Root root, Vector2Int position)
    {
        SetConnectivity(root, ConnectivityIfPlaced(position));
        AddToNeighborConnectivity(new Vector2Int(position.x, position.y + 1), Direction.south);
        AddToNeighborConnectivity(new Vector2Int(position.x, position.y - 1), Direction.north);
        AddToNeighborConnectivity(new Vector2Int(position.x + 1, position.y), Direction.west);
        AddToNeighborConnectivity(new Vector2Int(position.x - 1, position.y), Direction.east);
    }

    private Direction ConnectivityIfPlaced(Vector2Int position)
    {
        Vector2Int north = new Vector2Int(position.x, position.y + 1);
        Vector2Int south = new Vector2Int(position.x, position.y - 1);
        Vector2Int east = new Vector2Int(position.x + 1, position.y);
        Vector2Int west = new Vector2Int(position.x - 1, position.y);
        Direction[] positionsOccupied = new Direction[4]
        {
                GridMap.Current.IsCellOccupied<RootNetworkComponent>(north) ? Direction.north : Direction.none,
                GridMap.Current.IsCellOccupied<RootNetworkComponent>(south) ? Direction.south : Direction.none,
                GridMap.Current.IsCellOccupied<RootNetworkComponent>(east) ? Direction.east : Direction.none,
                GridMap.Current.IsCellOccupied<RootNetworkComponent>(west) ? Direction.west : Direction.none
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
        mapOfRoots[root.GetComponent<GridTransform>().topLeftPosMap.x, root.GetComponent<GridTransform>().topLeftPosMap.y] = false;
        Destroy(root.gameObject);
        //remove root from network
    }

    public bool GetRootPath(Vector2Int start, Vector2Int end, out List<Vector2Int> path)
    {
        return rootPathfinder.GetPath(start, end, out path);
    }

    //this needs to be fixed.
    //me 2 weeks later: why?
    //i think the idea is that...  we want to just use the graph numbers are equal thing.
    public bool RootBuildingsAreConnected(RootBuildingComponent b1, RootBuildingComponent b2)
    {
        return rootPathfinder.GetPath(b1.GetComponent<GridTransform>().topLeftPosMap, b2.GetComponent<GridTransform>().topLeftPosMap, out List<Vector2Int> _);
    }
}
