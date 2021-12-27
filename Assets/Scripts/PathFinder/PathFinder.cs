using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinder
{
    private static List<PFTile> openTiles = new List<PFTile>();
    private static List<PFTile> closedTiles = new List<PFTile>();
    private static PFGrid grid;
    private static readonly int horizontalScore = 10;
    private static readonly int diagonalScore = 14;

    private class PFTile
    {
        public Vector2Int position;
        public bool passable;
        public int blockerTracker;
        public int f_score;//total score -- equals g + h.  g is sum from start to current of actual distance traveled.
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
            if(passabl)
            {
                if(blockerTracker == 0)
                {
                    throw new InvalidOperationException("passability is already all clear. cannot double-update blocking status twice in the same direction");
                }
                else
                {
                    blockerTracker = blockerTracker >> 1; //removing a blocker using a shifter
                }
            }
            else
            {
                blockerTracker = (blockerTracker << 1) + 1;//adding a blocker by shifting in a new 1 - no check for max, but this could happen...
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
    public static void UpdatePassable(Vector2Int pos, bool passable)
    {
        if (grid.tiles[pos.x, pos.y] == null) Debug.Log("pftile is null");
        PFTile tile = grid.tiles[pos.x, pos.y];
        tile.UpdatePassable(passable);
        
    }
    
    public static void Initialize(int width, int height, bool[,] passableMap)
    {
        grid = new PFGrid(width, height);
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                grid.tiles[x, y] = new PFTile(x, y);
                grid.tiles[x, y].InitializePassable(passableMap[x, y]);
            }
        }
    }

    private static void Reset()
    {
        openTiles.Clear();
        closedTiles.Clear();
        for(int x = 0; x < grid.width; x++)
        {
            for(int y = 0; y < grid.height; y++)
            {
                PFTile tile = grid.tiles[x, y];
                if(tile == null) { Debug.Log("tile is null"); }
                tile.f_score = int.MaxValue;
                tile.g_score = int.MaxValue;
                tile.h_score = int.MaxValue;
                tile.previous = null;
            }
        }
    }

    //getpath
    public static List<Vector2Int> GetPath(Vector2Int start, Vector2Int end, bool adjacent = false)
    {
        Reset();
        List<Vector2Int> result = null;
        PFTile startTile = grid.tiles[start.x, start.y];
        openTiles.Add(startTile);
        while(openTiles.Count>0 && !closedTiles.Exists(x => x.position == end))
        {
            int currentF = int.MaxValue;
            PFTile currentTile = null;
            foreach(PFTile tile in openTiles)
            {
                if(tile.f_score <= currentF)
                {
                    currentF = tile.f_score;
                    currentTile = tile;
                }
            }
            openTiles.Remove(currentTile);
            closedTiles.Add(currentTile);
            List<PFTile> adjacencies = GetAdjacencies(currentTile);
            foreach(PFTile node in adjacencies)
            {
                if(!closedTiles.Contains(node) && node.passable)
                {
                    int tentative_g_score = CalculateGScore(currentTile, node);
                    if(closedTiles.Contains(node) && tentative_g_score >= node.g_score)
                    {
                        continue;
                    }
                    if(!openTiles.Contains(node) || tentative_g_score < node.g_score)
                    {
                        node.previous = currentTile;
                        node.g_score = tentative_g_score;
                        node.h_score = DistanceToTarget(node.position, end);
                        node.f_score = node.g_score + node.h_score;
                        if(!openTiles.Contains(node))
                        {
                            openTiles.Add(node);
                        }
                    }
                }
            }
        }
        if (!closedTiles.Exists(x => x.position == end))
        {
            return null;
        }
            
        else
        {
            result = new List<Vector2Int>();
        }

        PFTile temp = closedTiles.Find(x => x.position == end);
        while(temp != null)
        {
            result.Add(temp.position);
            temp = temp.previous;
        }
        result.Reverse();
        if(adjacent)
        {
            result.RemoveAt(result.Count - 1);
        }
        //construct the end path.
        return result;
    }

    private static int DistanceToTarget(Vector2Int start, Vector2Int target)
    {
        int horiz_dist = Math.Abs(target.x - start.x);
        int vertical_dist = Math.Abs(target.y - start.y);
        int max_val = Math.Max(vertical_dist, horiz_dist);
        int min_val = Math.Min(vertical_dist, horiz_dist);
        int straight_steps = max_val - min_val;
        int diag_steps = max_val - straight_steps;
        return straight_steps * horizontalScore + diag_steps * diagonalScore;
    }

    private static int CalculateGScore(PFTile current, PFTile adjacent)
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

    private static List<PFTile> GetAdjacencies(PFTile current)
    {
        int x = current.position.x;
        int y = current.position.y;
        bool up = false, down = false, right = false, left = false;
        List<PFTile> result = new List<PFTile>();

        if(x>0)
        {
            left = TestTileAndAddToList(x - 1, y, result);
        }
        if(x<grid.width-1)
        {
            right = TestTileAndAddToList(x + 1, y, result);
        }
        if(y>0)
        {
            down = TestTileAndAddToList(x, y - 1, result);
        }
        if(y<grid.height-1)
        {
            up = TestTileAndAddToList(x, y + 1, result);
        }
        if(left&&up && grid.tiles[x-1,y+1].passable)
        {
            result.Add(grid.tiles[x - 1, y + 1]);
        }
        if(right&&up && grid.tiles[x+1,y+1].passable)
        {
            result.Add(grid.tiles[x + 1, y + 1]);
        }
        if (left && down && grid.tiles[x - 1, y - 1].passable)
        {
            result.Add(grid.tiles[x - 1, y - 1]);
        }
        if (right && down && grid.tiles[x + 1, y - 1].passable)
        {
            result.Add(grid.tiles[x + 1, y - 1]);
        }
        return result;
    }

    private static bool TestTileAndAddToList(int x, int y, List<PFTile> tileList)
    {
        if (grid.tiles[x, y].passable)
        {
            tileList.Add(grid.tiles[x, y]);
            return true;
        }
        return false;
    }
}
