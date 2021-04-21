using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//move to another file possibly... put this delegate in the uImanager, send the function
public delegate void OnSetButton(Building building);

public class SetInputBuildingButton : MonoBehaviour
{
    public UIManager uIManager;
    public Building building;

    public void OnSetInputBuilding(Building bldg)
    {
        building.ConnectForTransfer(bldg, bldg.ResourceProduction);
    }

    public void OnClick()
    {
        uIManager.StartSelectBuilding(OnSetInputBuilding);
    }
}
