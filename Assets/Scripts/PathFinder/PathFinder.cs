using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class PathFinder
{
    
    private NeighborType neighborType;
    private FastPriorityQueue<PFTile> openTiles;
    private HashSet<PFTile> closedTiles;
    private PFGrid grid;
    private readonly int horizontalScore = 10;
    private readonly int diagonalScore = 14;

    private class PFTile : FastPriorityQueueNode
    {
        public Vector2Int position;
        public bool passable;
        public int blockerTracker;
        private int f_score;//total score -- equals g + h.  g is sum from start to current of actual distance traveled.
        public void SetFScore(int inScore, FastPriorityQueue<PFTile> openTiles)
        {
            f_score = inScore;
            if (openTiles.Contains(this))
            {
                openTiles.UpdatePriority(this, f_score);
            }
            else
            {
                Priority = inScore;
            }
        }
        public int F_score
        {
            get => f_score;
        }
        public int g_score;//summed distance score
        public int h_score;//heuristic from cell to destination
        public PFTile previous;

        public PFTile(int x, int y)
        {
            position.x = x;
            position.y = y;
        }

        public void InitializePassable(bool passabl)
        {
            passable = passabl;
            blockerTracker = passable ? 0 : 1;
        }

        public void UpdatePassable(bool passabl)
        {
            if (passabl)
            {
                if (blockerTracker == 0)
                {
                    throw new InvalidOperationException("passability is already all clear. cannot double-update blocking status twice in the same direction");
                }
                else
                {
                    blockerTracker--;
                }
            }
            else
            {
                blockerTracker++;//adding a blocker by shifting in a new 1 - no check for max, but this could happen...
            }
            passable = blockerTracker == 0;
        }

        public void ClearPassable()
        {
            blockerTracker = 0;
            passable = true;
        }
    }

    private class PFGrid
    {
        public int width;
        public int height;
        public PFTile[,] tiles;
        public PFGrid(int width, int height)
        {
            tiles = new PFTile[width, height];
            this.width = width;
            this.height = height;
        }
    }

    //???
    public void UpdatePassable(Vector2Int pos, bool passable)
    {
        if (grid.tiles[pos.x, pos.y] == null) Debug.Log("pftile is null");
        PFTile tile = grid.tiles[pos.x, pos.y];
        tile.UpdatePassable(passable);

    }

    public PathFinder(int width, int height, bool[,] passableMap, NeighborType nType = NeighborType.eightWay)
    {
        Initialize(width, height, passableMap, nType);
    }

    public void Initialize(int width, int height, bool[,] passableMap, NeighborType nType = NeighborType.eightWay)
    {
        grid = new PFGrid(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid.tiles[x, y] = new PFTile(x, y);
                grid.tiles[x, y].InitializePassable(passableMap[x, y]);
            }
        }
        openTiles = new FastPriorityQueue<PFTile>(width * height);//hang on... do we even need a priority queue?  can't we simplify this if we just use position??
        closedTiles = new HashSet<PFTile>();
        neighborType = nType;
    }

    private void Reset()
    {
        for (int x = 0; x < grid.width; x++)
        {
            for (int y = 0; y < grid.height; y++)
            {
                PFTile tile = grid.tiles[x, y];
                //if(tile == null) { Debug.Log("tile is null"); }
                tile.SetFScore(int.MaxValue, openTiles);
                tile.g_score = int.MaxValue;
                tile.h_score = int.MaxValue;
                tile.previous = null;
            }
        }
        openTiles.Clear();
        closedTiles.Clear();
    }

    //getpath
    public bool GetPath(Vector2Int start, Vector2Int end, out List<Vector2Int> result)
    {
        Reset();
        result = new List<Vector2Int>();
        PFTile startTile = grid.tiles[start.x, start.y];
        startTile.g_score = 0;
        PFTile endTile = grid.tiles[end.x, end.y];
        PFTile lowestHTile = startTile;
        int lowestHNum = int.MaxValue;
        openTiles.Enqueue(startTile);
        while (openTiles.Count > 0 && !closedTiles.Contains(endTile))
        {
            PFTile currentTile = null;
            currentTile = openTiles.Dequeue();
            //openTiles.ResetNode(currentTile);
            closedTiles.Add(currentTile);
            SetAdjacencies(currentTile);
            for (int i = 0; i < 8; i++)
            {
                PFTile node = adjacencies[i];
                if (node == null) continue;
                if (!closedTiles.Contains(node) && node.passable)
                {
                    int tentative_g_score = CalculateGScore(currentTile, node);
                    if (closedTiles.Contains(node) && tentative_g_score >= node.g_score)
                    {
                        continue;
                    }
                    if (!openTiles.Contains(node) || tentative_g_score < node.g_score)
                    {
                        node.previous = currentTile;
                        node.g_score = tentative_g_score;
                        node.h_score = DistanceToTarget(node.position, end);
                        node.SetFScore(node.g_score + node.h_score, openTiles);
                        if (!openTiles.Contains(node))
                        {
                            openTiles.Enqueue(node);
                        }
                        if (node.h_score < lowestHNum)
                        {
                            lowestHTile = node;
                            lowestHNum = node.h_score;
                        }
                    }
                }
            }
        }
        bool foundPath = closedTiles.Contains(endTile);
        PFTile tempTile;
        tempTile = lowestHTile;

        while (tempTile != null)
        {
            result.Add(tempTile.position);
            tempTile = tempTile.previous;
        }
        result.Reverse();

        //construct the end path.
        return foundPath;
    }

    private int DistanceToTarget(Vector2Int start, Vector2Int target)
    {
        int horiz_dist = Math.Abs(target.x - start.x);
        int vertical_dist = Math.Abs(target.y - start.y);
        int max_val = Math.Max(vertical_dist, horiz_dist);
        int min_val = Math.Min(vertical_dist, horiz_dist);
        int straight_steps = max_val - min_val;
        int diag_steps = max_val - straight_steps;
        return straight_steps * horizontalScore + diag_steps * diagonalScore;
    }

    private int CalculateGScore(PFTile current, PFTile adjacent)
    {
        int xDist = Math.Abs(current.position.x - adjacent.position.x);
        int yDist = Math.Abs(current.position.y - adjacent.position.y);
        int ManhattanDistance = xDist + yDist;
        if (ManhattanDistance == 2)
        {
            //Debug.Log("we are getting the diagonal scores");
            return current.g_score + diagonalScore;
        }

        else return current.g_score + horizontalScore;
    }

    private PFTile[] adjacencies = new PFTile[8];

    private void SetAdjacencies(PFTile current)
    {
        int x = current.position.x;
        int y = current.position.y;
        bool up = false, down = false, right = false, left = false;

        if (x > 0)
        {
            left = TestTileAndAddToAdjacencies(x - 1, y, 0);
        }
        if (x < grid.width - 1)
        {
            right = TestTileAndAddToAdjacencies(x + 1, y, 1);
        }
        if (y > 0)
        {
            down = TestTileAndAddToAdjacencies(x, y - 1, 2);
        }
        if (y < grid.height - 1)
        {
            up = TestTileAndAddToAdjacencies(x, y + 1, 3);
        } //add the if for neighbortype...
        adjacencies[4] = null;
        adjacencies[5] = null;
        adjacencies[6] = null;
        adjacencies[7] = null;
        if (neighborType == NeighborType.eightWay)
        {
            if (left && up && grid.tiles[x - 1, y + 1].passable)
            {
                adjacencies[4] = grid.tiles[x - 1, y + 1];
            }
            if (right && up && grid.tiles[x + 1, y + 1].passable)
            {
                adjacencies[5] = grid.tiles[x + 1, y + 1];
            }
            if (left && down && grid.tiles[x - 1, y - 1].passable)
            {
                adjacencies[6] = grid.tiles[x - 1, y - 1];
            }
            if (right && down && grid.tiles[x + 1, y - 1].passable)
            {
                adjacencies[7] = grid.tiles[x + 1, y - 1];
            }
        }
    }

    private bool TestTileAndAddToAdjacencies(int x, int y, int index)
    {
        if (grid.tiles[x, y].passable)
        {
            adjacencies[index] = grid.tiles[x, y];
            return true;
        }
        adjacencies[index] = null;
        return false;
    }
}
