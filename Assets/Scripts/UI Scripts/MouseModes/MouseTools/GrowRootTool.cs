using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowRootTool : MouseTool
{
    private RootCursor _rootCursor;

    private static RootManager rootManager;

    private static RootBuildingComponent rootBuilding;

    private static GrowRootTool _instance;
    public static GrowRootTool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GrowRootTool();
                rootManager = RootManager.Instance;
            }
            return _instance;
        }
    }

    public GrowRootTool()
    {
        Debug.Log("instantiating");
        _rootCursor = Object.Instantiate(PrefabReferences.Instance.RootCursor, GridMap.Current.transform);
        _rootCursor.gameObject.SetActive(false);
    }

    public override bool LeftClick(Vector3 mousePosition)
    {
        Vector2Int mapCoords = GridMap.Current.WorldToMap(mousePosition);
        if (GridMap.Current.IsWithinBounds(mapCoords))
        {
            rootBuilding.SetRootGrowthTarget(mapCoords);
            return false;
        }
        return true;
    }

    public override bool RightClick(Vector3 mousePosition)
    {
        return false;
    }

    //unityobject1 is going to be the building...
    public override void StartTool(Object unityObject1 = null, Object unityObject2 = null)
    {
        rootBuilding = (RootBuildingComponent)unityObject1;
        _rootCursor.gameObject.SetActive(true);
        _rootCursor.enableCursor();

    }

    public override void EndTool()
    {
        _rootCursor.disableCursor();
        _rootCursor.gameObject.SetActive(false);
    }
}
