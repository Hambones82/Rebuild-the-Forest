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

    private bool[,] rootPassableMap;//map for the roots to grow?
    private PathFinder rootPathfinder;
    private bool[,] mapOfRoots;
    private PathFinder mapOfRootsPathfinder;
    
    [SerializeField]
    private Sprite[] sprites;

    private List<RootBuildingComponent> rootBuildings;

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
        mapOfRoots = new bool[width, height];
        for(int x=0; x<width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                mapOfRoots[x, y] = GridMap.Current.IsCellOccupied(new Vector2Int(x, y), MapLayer.roots)
                    || GridMap.Current.IsCellOccupied<RootBuildingComponent>(new Vector2Int(x, y), MapLayer.buildings);
            }
        }
        rootBuildings = new List<RootBuildingComponent>();
        foreach(Building building in BuildingManager.Instance.Buildings)
        {
            RootBuildingComponent rbc = building.GetComponent<RootBuildingComponent>();
            if(rbc != null)
            {
                rootBuildings.Add(rbc);
            }
        }
        //then, need to set the networks for the rbcs... test for connectivity, etc...
        mapOfRootsPathfinder = new PathFinder(width, height, mapOfRoots, PathFinder.NeighborType.fourWay);
        /*
        List<RootBuildingComponent> openList = new List<RootBuildingComponent>(rootBuildings);
        List<RootBuildingComponent> closedList = new List<RootBuildingComponent>();//might not need the closed list.
        int currentNetwork = 0;
        while(openList.Count > 0)
        {
            RootBuildingComponent currentRBC = openList[0];
            openList.Remove(currentRBC);
            closedList.Add(currentRBC);
            currentRBC.RootNetwork = currentNetwork;
            foreach(RootBuildingComponent candidateRBC in openList)
            {
                if(mapOfRootsPathfinder.GetPath(currentRBC.GetComponent<GridTransform>().topLeftPosMap, 
                    candidateRBC.GetComponent<GridTransform>().topLeftPosMap, out List<Vector2Int> _))
                {
                    candidateRBC.RootNetwork = currentNetwork;
                    openList.Remove(candidateRBC);
                    closedList.Add(candidateRBC);
                }
            }
            currentNetwork++;
            
        }//don't we also want to assign */
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
            mapOfRoots[position.x, position.y] = true;
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
        mapOfRoots[root.GetComponent<GridTransform>().topLeftPosMap.x, root.GetComponent<GridTransform>().topLeftPosMap.y] = false;
        Destroy(root.gameObject);
        
    }

    public bool GetRootPath(Vector2Int start, Vector2Int end, out List<Vector2Int> path)
    {
        return rootPathfinder.GetPath(start, end, out path);
    }

    //this needs to be fixed.
    public bool RootBuildingsAreConnected(RootBuildingComponent b1, RootBuildingComponent b2)
    {
        return rootPathfinder.GetPath(b1.GetComponent<GridTransform>().topLeftPosMap, b2.GetComponent<GridTransform>().topLeftPosMap, out List<Vector2Int> _);
    }
}
