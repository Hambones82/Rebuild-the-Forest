using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNetworkComponent : MonoBehaviour, IMGNNode
{
    private GridTransform gridTransform;

    [SerializeField]
    private int _networkID;

    private void Awake()
    {
        gridTransform = GetComponent<GridTransform>();
    }

    public int NetworkID
    {
        get => _networkID;
        set => _networkID = value;
    }

    public List<IMGNNode> GetAdjacentNodes()
    {
        List<IMGNNode> retVal = new List<IMGNNode>();
        List<Vector2Int> adjacentTiles = gridTransform.GetAdjacentTiles(NeighborType.fourWay);
        //Debug.Log($"current object pos: {gridTransform.topLeftPosMap}");
        
        foreach(Vector2Int pos in adjacentTiles)
        {
            //Debug.Log($"for neighbor {i++}: {pos}");
            RootNetworkComponent rootAtPos = GridMap.Current.GetObjectAtCell<RootNetworkComponent>(pos);
            if(rootAtPos != null && !retVal.Contains(rootAtPos))
            {
                retVal.Add(rootAtPos);
            }
        }
        /*
        foreach(IMGNNode node in retVal)
        {
            Debug.Log($"{node.NetworkID}");
        }*/
        return retVal;
    }


    //either store one per building or store one for each building tile.  
    //if store one per tile, reltaively easy, but some duplication of data.
    //if store one per building, adjacency check is complicated.
}
