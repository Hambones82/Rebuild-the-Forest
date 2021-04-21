using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandWindowInvoker : MonoBehaviour {

    private UIManager uiManager;
    
    public CommandPanel commandPanelPrefab;

    public void SetUIManager(UIManager uiManagerT)
    {
        uiManager = uiManagerT;
    }

    public void InvokeCommandWindow()
    {
        uiManager.OpenBuildingPanel(GetComponent<Building>());
    }

    public void CloseCommandWindow()
    {
        uiManager.CloseBuildingPanel(GetComponent<Building>());
    }
}
