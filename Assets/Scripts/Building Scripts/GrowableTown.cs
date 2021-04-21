using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Building))]
public class GrowableTown : MonoBehaviour
{
    public int growthLevel = 0;

    public List<GrowableTownLevel> growableTownLevels;

    private Building _building;

    
    public void Start()
    {
        ValidateAndSortLevels();
        _building = GetComponent<Building>();
        _building.endTurnCallbacks += TownEndTurn;
    }

    private void ValidateAndSortLevels()
    {
        growableTownLevels.Sort((p, q) => p.levelNumber.CompareTo(q.levelNumber));

    }   

    public void TownEndTurn()
    {
        //depending on current state...  go up or down...  so maybe will need states?  
        int newGrowthLevel = DetermineAppropriateLevel();
        if(newGrowthLevel != growthLevel)
        {
            ChangeGrowthLevel(newGrowthLevel);
        }

    }

    public int DetermineAppropriateLevel()
    {
        int retVal = 0;
        for(int i = growableTownLevels.Count-1; i>=0; i--)
        {
            if (_building.totalIntake.HasAtLeast(growableTownLevels[i].levelRequirements))
            {
                retVal = i;
                break;
            }
        }
        Debug.Log($"correct building level is {retVal}");
        return retVal;
    }

    public void ChangeGrowthLevel(int toLevel)
    {
        growthLevel = toLevel;
        _building.IntakeRequirements = growableTownLevels[toLevel].levelRequirements;
        _building.ResourceProduction = growableTownLevels[toLevel].buildingOutput;
        GetComponent<SpriteRenderer>().sprite = growableTownLevels[toLevel].levelSprite;
    }
    
}
