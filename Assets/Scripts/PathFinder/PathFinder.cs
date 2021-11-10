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
        public int f_score;//total score -- equals g + h.  g is sum from start to current of actual distance traveled.
        public int g_score;//summed distance score
        public int h_score;//heuristic from cell to destination
        public PFTile previous;

        public PFTile(int x, int y, bool passabl)
        {
            position.x = x;
            position.y = y;
            passable = passabl;
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

    //initialize
    public static void Initialize(int width, int height, bool[,] passableMap)
    {
        grid = new PFGrid(width, height);
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                grid.tiles[x, y] = new PFTile(x, y, passableMap[x,y]);
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
                grid.tiles[x, y].f_score = int.MaxValue;
                grid.tiles[x, y].g_score = int.MaxValue;
                grid.tiles[x, y].h_score = int.MaxValue;
                grid.tiles[x, y].previous = null;
            }
        }
    }
    //update passable - need a hookup for this


    //getpath
    public static List<Vector2Int> GetPath(Vector2Int start, Vector2Int end)
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
                    if(!openTiles.Contains(node))
                    {
                        node.previous = currentTile;
                        node.h_score = DistanceToTarget(node.position, end);
                        node.g_score = CalculateGScore(currentTile, node);
                        node.f_score = node.h_score + node.g_score;
                        openTiles.Add(node);
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
        if (ManhattanDistance == 2) return current.g_score + diagonalScore;
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
