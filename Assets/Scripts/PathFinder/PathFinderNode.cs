using UnityEngine;
using Priority_Queue;

public class PathFinderNode : FastPriorityQueueNode
{
    public Vector2Int position;
    public PathFinderNode(int x, int y)
    {
        position.x = x;
        position.y = y;
    }
}