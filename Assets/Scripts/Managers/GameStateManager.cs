using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DefaultExecutionOrder(0)]
public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    List<PollutionSource> pollutionSources;
    [SerializeField]
    ActorUnitManager unitManager;
    [SerializeField]
    PollutionManager pollutionManager;
    [SerializeField]
    GridMap gridMap;

    
    //get references to all
    private void Awake()
    {
        pollutionSources = FindObjectsOfType<PollutionSource>().ToList();
        BuildingManager.Instance.OnBuildingDelete += BuildingIsDeleted;
        unitManager = FindObjectOfType<ActorUnitManager>();
        unitManager.OnActorUnitDeath += ActorUnitDies;
        pollutionManager = FindObjectOfType<PollutionManager>();
        pollutionManager.OnPollutionAdded += PollutionIsAdded;
        gridMap = FindObjectOfType<GridMap>();        
    }

    private void ActorUnitDies(ActorUnit actorUnit)
    {
        //Debug.Log($"actor unit dies, number: {unitManager.CurrentActorUnitsCount}");
        if(unitManager.CurrentActorUnitsCount == 0)
        {
            Lose();
        }
    }

    private void BuildingIsDeleted(Building building)
    {
        PollutionSource pSource = building.GetComponent<PollutionSource>();
        if (pSource != null)
        {
            if(pollutionSources.Contains(pSource))
            {
                pollutionSources.Remove(pSource);
            }
            if (pollutionSources.Count == 0)
            {
                Win();
            }
        }
        
    }

    private void PollutionIsAdded(Vector2Int cell)
    {
        //check if it's on the cleaner building.  if so, check that the entire cleaner building is covered, then call lose if so...
        BuildingComponentCleaner cleaner = 
            gridMap.GetObjectAtCell<BuildingComponentCleaner>(cell, MapLayer.buildings);
        if(cleaner != null)
        {
            //check if all of it is covered...
            bool allIsCovered = true;
            RectInt cleanerArea = cleaner.GetComponent<GridTransform>().GetRect();
            foreach(Vector2Int pos in cleanerArea.allPositionsWithin)
            {
                if(gridMap.GetObjectsAtCell(pos, MapLayer.pollution).Count == 0)
                {
                    allIsCovered = false;
                    break;
                }
            }
            if(allIsCovered)
            {
                Lose();
            }
        }
    }

    private void Win() 
    {
        Debug.Log("You win");
    }

    private void Lose()
    {
        Debug.Log("You lose");
    }
}
