using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoObjectSelected : MouseMode
{
    private static NoObjectSelected _instance;
    public static NoObjectSelected Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new NoObjectSelected();
            }
            return _instance;
        }
    }

    public override MouseMode LeftClick(Vector3 clickPoint, UIManager uiManager)
    {
        GridTransform target = uiManager.gridMap.GetClosestClickedObject(clickPoint);
            
        if (target != null) //if you click on something
        {
            uiManager.SelectGridTransform(target);
            return ObjectSelected.Instance; //mouse state is that something is selected
        }
        else
        {
            return Instance;
        }
    }

    public override MouseMode RightClick(Vector3 clickPoint, UIManager uiManager)
    {
        return Instance;
    }
}
