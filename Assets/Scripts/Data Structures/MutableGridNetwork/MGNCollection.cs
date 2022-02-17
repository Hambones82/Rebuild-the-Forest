using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGNCollection<T> where T : IMGNNode
{
    private Dictionary<int, MutableGridNetwork<T>> networks = new Dictionary<int, MutableGridNetwork<T>>();
    private int lowestFreeNetworkID = 0;

    public void AddNode(T node)
    {
        //3 cases: no neighbors, some neighbors, all same network, some neighbors of different networks
        List<IMGNNode> neighbors = node.GetAdjacentNodes();
        bool oneValidNeighbor = false;
        IMGNNode validNeighbor = null;
        foreach(IMGNNode iNode in neighbors)
        {
            if(iNode.IsValid())
            {
                oneValidNeighbor = true;
                validNeighbor = iNode;
                break;
            }
        }
        if(neighbors.Count == 0 || !oneValidNeighbor)
        {
            //Debug.Log("no neighbors");
            int networkNum = NewNetwork();
            networks[networkNum].AddNode(node);
            node.NetworkID = networkNum;
        }
        else if(neighbors.Count == 1)
        {
            //add to old network
            networks[validNeighbor.NetworkID].AddNode(node);
        }
        else
        {
            //check for conflict
            //conflict occurs if there are at least two different ones\
            //Debug.Log($"neighbors count: {neighbors.Count}");
            int sameID = validNeighbor.NetworkID;
            
            for(int i = 1; i < neighbors.Count; i++)
            {
                int neighborID = neighbors[i].NetworkID;
                if(neighborID != sameID)
                {
                    //there's a conflict
                    networks[neighborID].AddToNetwork(networks[sameID]);
                }
            }

            networks[sameID].AddNode(node);
        }
    }

    public int NewNetwork()
    {
        UpdateLowestFreeNetworkID();
        return lowestFreeNetworkID;
    }

    private void UpdateLowestFreeNetworkID()
    {
        lowestFreeNetworkID = 0;
        
        while(true)//probably asking for it?
        {
            bool exists = networks.TryGetValue(lowestFreeNetworkID, out MutableGridNetwork<T> mgn);
            if(exists)
            {
                if (mgn.IsEmpty()) return;
                else //the network at the current ID is not empty, so increment lowestID
                {
                    lowestFreeNetworkID++;
                }
            }
            else //if(!exists) -- meaning there is no key for lowest free networkid, then that's the lowest free
            {
                networks[lowestFreeNetworkID] = new MutableGridNetwork<T>(lowestFreeNetworkID);
                return;
            }

        }
    }
}
