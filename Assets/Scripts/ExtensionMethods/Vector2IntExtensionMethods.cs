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
}
