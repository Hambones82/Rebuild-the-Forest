using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DefaultExecutionOrder(-7)]
public class GameStateManager : MonoBehaviour
{
    [SerializeField]
    List<BuildingComponentPollutionSource> pollutionSources;
    //get references to all
    private void Awake()
    {
        pollutionSources = FindObjectsOfType<BuildingComponentPollutionSource>().ToList();
        BuildingManager.Instance.OnBuildingDelete += BuildingIsDeleted;
    }

    private void BuildingIsDeleted(Building building)
    {
        BuildingComponentPollutionSource pSource = building.GetComponent<BuildingComponentPollutionSource>();
        if (pSource != null)
        {
            if(pollutionSources.Contains(pSource))
            {
                pollutionSources.Remove(pSource);
            }
        }
        if(pollutionSources.Count == 0)
        {
            Win();
        }
    }

    private void Win()
    {
        Debug.Log("You win");
    }

    private void Lose()
    {

    }
}
