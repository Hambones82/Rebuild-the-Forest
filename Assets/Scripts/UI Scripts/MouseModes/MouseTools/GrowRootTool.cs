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
        _rootCursor = Object.Instantiate(PrefabReferences.Instance.RootCursor, GridMap.Current.transform);
        _rootCursor.gameObject.SetActive(false);
    }

    public override bool LeftClick(Vector3 mousePosition)
    {
        rootBuilding.SetRootGrowthTarget(_rootCursor.GetComponent<GridTransform>().topLeftPosMap);
        return false;
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
        _rootCursor.Initialize(rootBuilding.transform.position, rootBuilding.RootRange);
    }

    public override void EndTool()
    {
        _rootCursor.disableCursor();
        _rootCursor.gameObject.SetActive(false);
    }
}
