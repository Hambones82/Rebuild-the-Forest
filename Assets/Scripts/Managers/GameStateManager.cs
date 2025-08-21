using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class GameStateManager : MonoBehaviour, IGameManager
{
    [SerializeField]
    List<PollutionSource> pollutionSources;
    [SerializeField]
    private ActorUnitManager _unitManager;
    [SerializeField]
    private PollutionManager _pollutionManager;
    [SerializeField]
    private GridMap _gridMap;
    [SerializeField]
    private BuildingManager _buildingManager;

    private ServiceLocator _serviceLocator;
    
    public void SelfInit(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null) throw new ArgumentNullException("service locator cannot be null");
        _serviceLocator = serviceLocator;
        _serviceLocator.RegisterService(this);
    }

    public void MutualInit()
    {
        pollutionSources = FindObjectsOfType<PollutionSource>().ToList();
        _buildingManager = _serviceLocator.LocateService<BuildingManager>();
        _unitManager = _serviceLocator.LocateService<ActorUnitManager>();
        _pollutionManager = _serviceLocator.LocateService<PollutionManager>();
        _buildingManager.OnBuildingDelete += BuildingIsDeleted;        
        _unitManager.OnActorUnitDeath += ActorUnitDies;        
        _pollutionManager.OnPollutionAdded += PollutionIsAdded;
        _gridMap = FindObjectOfType<GridMap>();
    }

    private void ActorUnitDies(ActorUnit actorUnit)
    {
        //Debug.Log($"actor unit dies, number: {unitManager.CurrentActorUnitsCount}");
        if(_unitManager.CurrentActorUnitsCount == 0)
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
            _gridMap.GetObjectAtCell<BuildingComponentCleaner>(cell, MapLayer.buildings);
        if(cleaner != null)
        {
            //check if all of it is covered...
            bool allIsCovered = true;
            RectInt cleanerArea = cleaner.GetComponent<GridTransform>().GetRect();
            foreach(Vector2Int pos in cleanerArea.allPositionsWithin)
            {
                if(_gridMap.GetObjectsAtCell(pos, MapLayer.pollution).Count == 0)
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
