using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootBuildingComponent : MonoBehaviour
{
    //function to set target for root growth
    //function to grow the roots
    //i guess that's it for now...
    [SerializeField]
    Vector2Int target;
    [SerializeField]
    List<Vector2Int> rootGrowthPath;
    [SerializeField]
    int rootGrowthProgress = 0;
    [SerializeField]
    bool positionSet = false;

    public void SetRootGrowthTarget(Vector2Int inTarget)
    {
        target = inTarget;
        rootGrowthProgress = 0; //but... don't we want to look at ... like the current progress, try to match it up to that???

    }


}
