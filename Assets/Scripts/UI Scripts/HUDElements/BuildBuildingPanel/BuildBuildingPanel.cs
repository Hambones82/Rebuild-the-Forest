using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//remove actor unit reference after building.  think about exactly how this works...  need to figure out what layer needs to do this
//also need to ensure the correct order.  make sure the buttons don't dance.

public class BuildBuildingPanel : ScrollableContentPanel
{
    private List<BuildingType> permittedBuildingTypes = new List<BuildingType>();
    private List<BuildingType> cachedPermittedBuildingTypes = new List<BuildingType>();
    protected override void UpdateContents()
    {
        List<BuildingType> availableBuildingTypes = BuildingManager.Instance.GetAvailableBuildingTypes();

        cachedPermittedBuildingTypes.Clear();
        foreach (BuildingType bType in permittedBuildingTypes)
        {
            cachedPermittedBuildingTypes.Add(bType);
        }
        permittedBuildingTypes.Clear();
        if (currentActorUnit != null)
        {
            for (int i = 0; i < availableBuildingTypes.Count; i++)
            {
                
                BuildingType bType = availableBuildingTypes[i];
                if (currentActorUnit.Stats.Stats.HasAtLeast(bType.BuildingPrefab.StatRequirements))//perhaps replace this with just "can build"?  i guess stat req is ok.
                {
                    permittedBuildingTypes.Add(bType);
                }
            }
            foreach (BuildingType bType in cachedPermittedBuildingTypes)
            {
                if (!permittedBuildingTypes.Contains(bType))
                {
                    //removebutton(bType)
                }
            }
            foreach (BuildingType bType in permittedBuildingTypes)
            {
                if (!cachedPermittedBuildingTypes.Contains(bType))
                {
                    AddButton(bType);
                }
            }
        }
    }

    protected override void ClearButtons()
    {
        base.ClearButtons();
        permittedBuildingTypes.Clear();
        cachedPermittedBuildingTypes.Clear();
    }

    private BuildBuildingButton AddButton(BuildingType bType)
    {
        BuildBuildingButton button = contentObjectPool.GetGameObject().GetComponent<BuildBuildingButton>();
        button.BType = bType;
        button.ActorUnit = currentActorUnit;
        button.Text.text = bType.BuildingName;
        button.gameObject.SetActive(true);
        button.transform.SetAsFirstSibling();
        contentObjects.Add(button.gameObject);
        return button;
    }
}
