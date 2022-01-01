using Unity.Profiling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public static class OldPathFinder
{
    //private static PathFinderNode[,] pathFinderNodeList;
    private static PathFinderPriorityQueue openTiles;
    private static HashSet<Vector2Int> closedTiles;
    private static PFGrid grid;
    private static readonly int horizontalScore = 10;
    private static readonly int diagonalScore = 14;

    private struct PFTile
    {
        public Vector2Int position;
        public bool passable;
        public int blockerTracker;
        private int f_score;//total score -- equals g + h.  g is sum from start to current of actual distance traveled.
        public int F_score
        {
            get => f_score;
            set
            {
                f_score = value;
                if(openTiles.Contains(position))
                {
                    openTiles.UpdatePriority(position, f_score);
                }
            }
        }
        public int g_score;//summed distance score
        public int h_score;//heuristic from cell to destination
        public Vector2Int previous; 

        public PFTile(int x, int y)
        {
            position = new Vector2Int(x,y);
            passable = false;
            blockerTracker = 0;
            f_score = int.MaxValue;
            g_score = int.MaxValue;
            h_score = int.MaxValue;
            previous = PathFinderPriorityQueue.noPosition;
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
    
    public static void UpdatePassable(Vector2Int pos, bool passable)
    {
        grid.tiles[pos.x, pos.y].UpdatePassable(passable);
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
        openTiles = new PathFinderPriorityQueue(width, height);//hang on... do we even need a priority queue?  can't we simplify this if we just use position??
        closedTiles = new HashSet<Vector2Int>();
    }
    //static readonly ProfilerMarker s_BeginReset = new ProfilerMarker("Reset");

    private static void Reset()
    {
        //s_BeginReset.Begin();
        openTiles.Clear();
        closedTiles.Clear();
        //s_BeginReset.End();
        //try to fix this loop that iterates over the whole thing...
        for (int x = 0; x < grid.width; x++)
        {
            for(int y = 0; y < grid.height; y++)
            {
                ref PFTile tile = ref grid.tiles[x, y];
                //if(tile == null) { Debug.Log("tile is null"); }
                tile.F_score = int.MaxValue;
                tile.g_score = int.MaxValue;
                tile.h_score = int.MaxValue;
                tile.previous = PathFinderPriorityQueue.noPosition;
            }
        }
        
    }

    //getpath
    public static bool GetPath(Vector2Int start, Vector2Int end, out List<Vector2Int> result)
    {
        Reset();
        result = new List<Vector2Int>();
        grid.tiles[start.x,start.y].g_score = 0;
        Vector2Int lowestHTile = start;
        int lowestHNum = int.MaxValue;
        openTiles.Enqueue(start.x, start.y, grid.tiles[start.x,start.y].F_score);
        while(openTiles.Count>0 && !closedTiles.Contains(end))
        {
            Vector2Int currentNodePosition = openTiles.DeQueue();
            closedTiles.Add(currentNodePosition);
            SetAdjacencies(currentNodePosition);
            ref PFTile currentNode = ref grid.tiles[currentNodePosition.x, currentNodePosition.y];
            for(int i = 0; i < 8; i++)
            {
                Vector2Int adjacentNodePosition = adjacencies[i];
                if (adjacentNodePosition == PathFinderPriorityQueue.noPosition) continue;
                ref PFTile adjacentNode = ref grid.tiles[adjacentNodePosition.x, adjacentNodePosition.y];
                if (!closedTiles.Contains(adjacentNodePosition) && adjacentNode.passable)
                {
                    int tentative_g_score = CalculateGScore(ref currentNode, ref adjacentNode);
                    if(closedTiles.Contains(adjacentNodePosition) && tentative_g_score >= adjacentNode.g_score)
                    {
                        continue;
                    }
                    if(!openTiles.Contains(adjacentNodePosition) || tentative_g_score < adjacentNode.g_score)
                    {
                        adjacentNode.previous = currentNode.position;
                        adjacentNode.g_score = tentative_g_score;
                        //Debug.Log($"node {node.position.position} g: {node.g_score}");
                        adjacentNode.h_score = DistanceToTarget(adjacentNode.position, end);
                        //Debug.Log($"node {node.position.position} h: {node.h_score}");
                        adjacentNode.F_score = adjacentNode.g_score + adjacentNode.h_score;
                        if(!openTiles.Contains(adjacentNodePosition))
                        {
                            openTiles.Enqueue(adjacentNodePosition.x, adjacentNodePosition.y, adjacentNode.F_score);
                        }
                        if (adjacentNode.h_score < lowestHNum)
                        {
                            lowestHTile = adjacentNodePosition;
                            lowestHNum = adjacentNode.h_score;
                        }
                    }
                }
            }
        }
        bool foundPath = closedTiles.Contains(end);
        //Debug.Log($"found path: {foundPath}");
        ref PFTile tempTile = ref grid.tiles[lowestHTile.x, lowestHTile.y];
        Vector2Int tempNode = tempTile.previous;
        
        while(tempNode != PathFinderPriorityQueue.noPosition)
        {
            result.Add(tempNode);
            tempTile = ref grid.tiles[tempNode.x, tempNode.y];
            tempNode = tempTile.previous;
        }
        result.Reverse();

        //construct the end path.
        return foundPath;
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

    private static int CalculateGScore(ref PFTile current, ref PFTile adjacent)
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

    private static Vector2Int[] adjacencies = new Vector2Int[8];

    private static void SetAdjacencies(Vector2Int current)
    {
        int x = current.x;
        int y = current.y;
        bool up = false, down = false, right = false, left = false;

        if(x>0)
        {
            left = TestTileAndAddToAdjacencies(x - 1, y, 0);
        }
        if(x<grid.width-1)
        {
            right = TestTileAndAddToAdjacencies(x + 1, y, 1);
        }
        if(y>0)
        {
            down = TestTileAndAddToAdjacencies(x, y - 1, 2);
        }
        if(y<grid.height-1)
        {
            up = TestTileAndAddToAdjacencies(x, y + 1, 3);
        }
        if(left&&up && grid.tiles[x-1,y+1].passable)
        {
            adjacencies[4] = new Vector2Int(x - 1, y + 1);
        }
        else
        {
            adjacencies[4] = PathFinderPriorityQueue.noPosition;
        }
        if(right&&up && grid.tiles[x+1,y+1].passable)
        {
            adjacencies[5] = new Vector2Int(x + 1, y + 1);
        }
        else
        {
            adjacencies[5] = PathFinderPriorityQueue.noPosition;
        }
        if (left && down && grid.tiles[x - 1, y - 1].passable)
        {
            adjacencies[6] = new Vector2Int(x - 1, y - 1);
        }
        else
        {
            adjacencies[6] = PathFinderPriorityQueue.noPosition;
        }
        if (right && down && grid.tiles[x + 1, y - 1].passable)
        {
            adjacencies[7] = new Vector2Int(x + 1, y - 1);
        }
        else
        {
            adjacencies[7] = PathFinderPriorityQueue.noPosition;
        }
    }

    private static bool TestTileAndAddToAdjacencies(int x, int y, int index)
    {
        if (grid.tiles[x, y].passable)
        {
            adjacencies[index] = new Vector2Int(x, y);
            return true;
        }
        adjacencies[index] = PathFinderPriorityQueue.noPosition;
        return false;
    }
}
