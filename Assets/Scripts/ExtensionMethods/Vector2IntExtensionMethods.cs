using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2IntExtensionMethods 
{
    public static List<Vector2Int> GetNeighbors(this Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        neighbors.Add(new Vector2Int(position.x - 1, position.y));
        neighbors.Add(new Vector2Int(position.x + 1, position.y));
        neighbors.Add(new Vector2Int(position.x, position.y + 1));
        neighbors.Add(new Vector2Int(position.x, position.y - 1));

        return neighbors;
    }

    public static List<Vector2Int> GetNeighborsInBounds(this Vector2Int position, int width, int height)
    {
        List<Vector2Int> neighborsInBounds = new List<Vector2Int>();
        if(position.x - 1 >= 0)
        {
            neighborsInBounds.Add(new Vector2Int(position.x - 1, position.y));
        }
        if(position.x + 1 < width)
        {
            neighborsInBounds.Add(new Vector2Int(position.x + 1, position.y));
        }
        if(position.y + 1 < height)
        {
            neighborsInBounds.Add(new Vector2Int(position.x, position.y + 1));
        }
        if(position.y - 1 >= 0)
        {
            neighborsInBounds.Add(new Vector2Int(position.x, position.y - 1));
        }
        return neighborsInBounds;
    }
}
