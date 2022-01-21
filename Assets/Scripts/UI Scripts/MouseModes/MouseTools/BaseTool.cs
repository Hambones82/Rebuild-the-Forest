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
    public override bool LeftClick(Vector3 mousePosition)
    {
        currentMouseMode = currentMouseMode.LeftClick(mousePosition);
        return true;
    }

    public override bool RightClick(Vector3 mousePosition)
    {
        currentMouseMode = currentMouseMode.RightClick(mousePosition);
        return true;
    }

    /*
    public override void Cancel()
    {
        
        UIManager.Instance.SelectGridTransform(null);
    }*/

    public override void StartTool(Object unityObject1 = null, Object unityObject2 = null)
    {
        
    }

    public override void EndTool()
    {
        currentMouseMode = NoObjectSelected.Instance;
    }
}
