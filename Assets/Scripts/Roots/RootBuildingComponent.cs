using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RootNetworkComponent))]
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
    float rootRange;
    public float RootRange { get => rootRange; }

    public delegate void RootChangeDelegate(Vector2Int target);
    public event RootChangeDelegate OnTargetChangeEvent;

    public event System.Action OnGrowthEnd;
    public event System.Action OnGrowthStart;
    
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
            OnTargetChangeEvent?.Invoke(target);
            OnGrowthStart?.Invoke();
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
    public bool Growing { get => positionSet; }
    [SerializeField]
    float time = 0;
    [SerializeField]
    float period;
    private void Update()
    {
        if(positionSet)
        {
            time += Time.deltaTime;
            if(time >= period)
            {
                if(AdvanceToNextOpenPosition())
                {
                    RootManager.Instance.SpawnRoot(rootGrowthPath[rootGrowthProgress]);
                    if(rootGrowthProgress == rootGrowthPath.Count - 1)
                    {
                        EndGrowth();
                    }
                }
                else
                {
                    EndGrowth();
                }
                time = 0;
            }
        }
    }

    private void EndGrowth()
    {

        positionSet = false;
        rootGrowthProgress = 0;
        OnGrowthEnd?.Invoke();
    }

    private bool AdvanceToNextOpenPosition()
    {
        while(rootGrowthProgress < rootGrowthPath.Count)
        {
            if(PositionIsOccupied(rootGrowthPath[rootGrowthProgress]))
            {
                rootGrowthProgress++;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    private bool PositionIsOccupied(Vector2Int position)
    {
        return GridMap.Current.IsCellOccupied(position, MapLayer.buildings)
            || GridMap.Current.IsCellOccupied(position, MapLayer.roots);
    }
}
