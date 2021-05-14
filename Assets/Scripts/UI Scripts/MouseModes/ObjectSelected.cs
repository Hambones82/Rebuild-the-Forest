using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelected : MouseMode
{
    private static ObjectSelected _instance;
    public static ObjectSelected Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ObjectSelected();
            }
            return _instance;
        }
    }

    public override MouseMode LeftClick(Vector3 clickPoint, UIManager uiManager)
    {
        GridTransform target = uiManager.gridMap.GetClosestClickedObject(clickPoint);
        if (target == uiManager.SelectedGridTransform)
        {
            return Instance;
        }
        else // either you click on another building or on nothing.
        {
            uiManager.SelectedGridTransform.GetComponent<MouseSelector>()?.DeSelect(); // therefore, you deselect the current selected one
            uiManager.OnDeselectEvent.Invoke();
            //here we use null as a valid value... maybe we should use something else?
            if (target != null)
            {
                uiManager.SelectGridTransform(target);
                return Instance;
            }
            else // clicked is null
            {
                uiManager.SelectGridTransform(null); // set to null because nothing is selected
                return NoObjectSelected.Instance;  
            }
        }
    }

    public override MouseMode RightClick(Vector3 clickPoint, UIManager uiManager)
    {
        ContextClickComponent contextClickComponent = uiManager.SelectedGridTransform.GetComponent<ContextClickComponent>();
        contextClickComponent?.DoContextClick(uiManager.gridMap.WorldToMap(clickPoint)); //this one...
        return Instance;
    }
}
