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

    private void Start()
    {
        SetRootGrowthTarget(new Vector2Int(35, 33));
    }
    //to test, set to 35,30.
    public void SetRootGrowthTarget(Vector2Int inTarget)
    {
        List<Vector2Int> result;
        if (RootManager.Instance.GetRootPath(GetComponent<GridTransform>().topLeftPosMap, inTarget, out result))
        {
            target = inTarget;
            rootGrowthProgress = 0; //but... don't we want to look at ... like the current progress, try to match it up to that???
            rootGrowthPath = result;
            positionSet = true;
            //root manager pathfinder.getpath to... from top left world pos... just remove from the first few the ones that are on the gridtransform...
        }
        else
        {
            positionSet = false;
            rootGrowthProgress = 0;
        }
    }

    [SerializeField]
    bool positionSet = false;
    [SerializeField]
    float time = 0;
    [SerializeField]
    float period;
    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.P))
        {
            SetRootGrowthTarget(new Vector2Int(35, 30));
        }
        if(positionSet)
        {
            time += Time.deltaTime;
            if(time >= period)
            {
                
                RootManager.Instance.SpawnRoot(rootGrowthPath[rootGrowthProgress++]);
                //need to set all the info for that root, but do it after we get the root grown...
                if(rootGrowthProgress >= rootGrowthPath.Count)
                {
                    positionSet = false;
                    rootGrowthProgress = 0;
                }
                time = 0;
            }
        }
    }

}
