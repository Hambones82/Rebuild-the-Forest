using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandPanel : MonoBehaviour {
    private Building building;

    //the place in which the content item prefab instances go
    public RectTransform textContentWindow;
    //content item prefab for spawning new info
    public CommandPanelTextElement textContentItemPrefab;
    public Toggle productionIsOnPrefab;
    private Toggle prodIsOnToggle;
    public SetInputBuildingButton setInputBuildingButtonPrefab;
    public UIManager uIManager;

    public void SetUIManager(UIManager uIM)
    {
        uIManager = uIM;
    }

    public void SetBuilding(Building buildingT)
    {
        building = buildingT;
        NewRow("requirements:", "");
        foreach(Resource r in building.IntakeRequirements.resources)
        {
            NewRow(r.resourceType.ToString(), r.currentAmount.ToString());
        }
        NewRow("production:", "");
        foreach(Resource r in building.ResourceProduction.resources)
        {
            NewRow(r.resourceType.ToString(), r.currentAmount.ToString());
        }
        Toggle newToggle = Instantiate(productionIsOnPrefab, textContentWindow.transform);
        newToggle.isOn = buildingT.productionIsOn;
        prodIsOnToggle = newToggle;
        newToggle.onValueChanged.AddListener(updateValueWhenClicked);
        SetInputBuildingButton setBuildingButton = Instantiate(setInputBuildingButtonPrefab, textContentWindow.transform);
        setBuildingButton.uIManager = uIManager;//
        setBuildingButton.building = building;
        //next is... button for get connection from...
    }

    public void updateValueWhenClicked(bool clickVal)
    {
        bool newVal = building.setProdIsOn(clickVal);
        prodIsOnToggle.SetIsOnWithoutNotify(newVal);
    }

    private CommandPanelTextElement NewRow(string label, string value)
    {
        CommandPanelTextElement retVal = Instantiate(textContentItemPrefab, textContentWindow.transform);
        retVal.SetLabel(label);
        retVal.SetValue(value);
        return retVal;
    }
}
