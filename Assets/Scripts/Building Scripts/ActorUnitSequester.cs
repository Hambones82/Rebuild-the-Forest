using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class ActorUnitSequester : MonoBehaviour
{
    public void RestoreActorUnit()
    {
        Debug.Log("restore function called");
        List<Vector2Int> candidateOpenPositions = GetComponent<GridTransform>().GetAdjacentTiles();
        List<Vector2Int> possiblePositions = new List<Vector2Int>();
        foreach (Vector2Int candidatePos in candidateOpenPositions)
        {
            if (PathingController.Instance.GetPassable(candidatePos))
            {
                possiblePositions.Add(candidatePos);
            }
        }
        if (possiblePositions.Count > 0 && ActorUnitManager.Instance.NumActorUnits < ActorUnitManager.Instance.MaxActorUnits)
        {
            ActorUnitManager.Instance.SpawnActorUnit(possiblePositions[Random.Range(0, possiblePositions.Count)]);
            BuildingManager.Instance.DestroyBuilding(GetComponent<Building>());
        }
    }
}
