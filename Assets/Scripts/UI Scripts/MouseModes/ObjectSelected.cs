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

    public override MouseMode LeftClick(Vector3 clickPoint)
    {
        GridTransform target = GridMap.Current.GetClosestClickedObject(clickPoint);
        if (target == UIManager.Instance.SelectedGridTransform)
        {
            return Instance;
        }
        else // either you click on another building or on nothing.
        {
            UIManager.Instance.SelectedGridTransform?.GetComponent<MouseSelector>()?.DeSelect(); // therefore, you deselect the current selected one
            UIManager.Instance.OnDeselectEvent.Invoke();
            //here we use null as a valid value... maybe we should use something else?
            if (target != null)
            {
                UIManager.Instance.SelectGridTransform(target);
                return Instance;
            }
            else // clicked is null
            {
                UIManager.Instance.SelectGridTransform(null); // set to null because nothing is selected
                return NoObjectSelected.Instance;  
            }
        }
    }

    public override MouseMode RightClick(Vector3 clickPoint)
    {
        ContextClickComponent contextClickComponent = UIManager.Instance.SelectedGridTransform.GetComponent<ContextClickComponent>();
        contextClickComponent?.DoContextClick(GridMap.Current.WorldToMap(clickPoint)); //this one...
        return Instance;
    }
}
