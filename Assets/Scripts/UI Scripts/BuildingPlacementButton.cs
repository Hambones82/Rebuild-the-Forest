using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacementButton : MonoBehaviour {

    public UIManager uiManager;
    public BuildingType buildingType;
    public PlayerResourceData playerData;

    public void ClickAction()
    {
        uiManager.StartPlacementByCursor(buildingType, playerData, true);
    }
}
