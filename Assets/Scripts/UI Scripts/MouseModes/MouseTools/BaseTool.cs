using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTool : MouseTool
{
    private static BaseTool _instance;
    public static BaseTool Instance
    {
        get
        {
            if (_instance == null)
                _instance = new BaseTool();
            return _instance;
        }
    }

    private MouseMode currentMouseMode = NoObjectSelected.Instance;
    public override bool LeftClick(Vector3 mousePosition, UIManager uiManager)
    {
        currentMouseMode = currentMouseMode.LeftClick(mousePosition, uiManager);
        return true;
    }

    public override bool RightClick(Vector3 mousePosition, UIManager uiManager)
    {
        currentMouseMode = currentMouseMode.RightClick(mousePosition, uiManager);
        return true;
    }
}
