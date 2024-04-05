using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEngine.UIElements;

public class ConnectivitySpriteUpdater<T> : MonoBehaviour where T : MonoBehaviour
{    
    [SerializeField] private Sprite[] sprites;


    private void OnEnable()
    {
        Vector2Int position = GetComponent<GridTransform>().topLeftPosMap;
        Vector2Int[] positions = GetAdjacentPositions(position);
        T[] connectedObjects = GetConnectedObjects(positions);
        Direction newConfiguration = GetDirection(connectedObjects);
        UpdateSprite(newConfiguration);
        UpdateNeighborSprites(positions, connectedObjects);                
    }

    private void UpdateNeighborSprites(Vector2Int[] positions, T[] connectedObjects)
    {
        for (int i = 0; i < 4; i++)
        {
            T neighbor = connectedObjects[i];
            if (neighbor != null)
            {
                Vector2Int[] neighborPositions = GetAdjacentPositions(positions[i]);
                T[] neighborConnectedObjects = GetConnectedObjects(neighborPositions);
                Direction coDirection = GetDirection(neighborConnectedObjects);
                neighbor.GetComponent<ConnectivitySpriteUpdater<T>>().UpdateSprite(coDirection);
            }
        }
    }

    private Vector2Int[] GetAdjacentPositions(Vector2Int position)
    {
        return new Vector2Int[4]
        {
            new Vector2Int(position.x, position.y + 1),
            new Vector2Int(position.x, position.y - 1),
            new Vector2Int(position.x + 1, position.y),
            new Vector2Int(position.x - 1, position.y)
        };
    }

    private T[] GetConnectedObjects(Vector2Int[] positions)
    {        
        T[] connectedObjects = new T[4]
        {
            GridMap.Current.GetObjectAtCell<T>(positions[0]),
            GridMap.Current.GetObjectAtCell<T>(positions[1]),
            GridMap.Current.GetObjectAtCell<T>(positions[2]),
            GridMap.Current.GetObjectAtCell<T>(positions[3])
        };
        return connectedObjects;
    }

    private Direction GetDirection(T[] connectedObjects)
    {
        Direction[] positionsOccupied = new Direction[4]
        {
                connectedObjects[0] != null ? Direction.north : Direction.none,
                connectedObjects[1] != null ? Direction.south : Direction.none,
                connectedObjects[2] != null ? Direction.east : Direction.none,
                connectedObjects[3] != null ? Direction.west : Direction.none
        };

        return positionsOccupied[0] | positionsOccupied[1] | positionsOccupied[2] | positionsOccupied[3];
    }

    private void UpdateSprite(Direction direction)
    {
        int directionCombination = (int)direction;
        GetComponent<SpriteRenderer>().sprite = sprites[directionCombination];
    }

    private void OnDisable()
    {
        Vector2Int[] positions = GetAdjacentPositions(GetComponent<GridTransform>().topLeftPosMap);
        T[] connectedObjects = GetConnectedObjects(positions);
        UpdateNeighborSprites(positions, connectedObjects);
    }
}
