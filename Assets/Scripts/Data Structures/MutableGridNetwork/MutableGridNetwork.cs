using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutableGridNetwork<T> where T : IMGNNode
{
    public List<T> nodes = new List<T>();
    public int networkID;

    public MutableGridNetwork(int ID)
    {
        networkID = ID;
    }

    public MutableGridNetwork(int ID, MutableGridNetwork<T> otherNetwork)
    {
        networkID = ID;
        nodes.AddRange(otherNetwork.nodes);
    }

    public void AddNode(T node)
    {
        nodes.Add(node);
        node.NetworkID = networkID;
    }

    public void AddToNetwork(MutableGridNetwork<T> mgn)
    {
        int ID = mgn.networkID;
        if(networkID != ID)
        {
            foreach (T node in nodes)
            {
                node.NetworkID = ID;
            }
            mgn.nodes.AddRange(nodes);
            nodes.Clear();
        }
    }

    public bool IsEmpty()
    {
        return nodes.Count == 0;
    }

    //need to do remove, but let's check creation and add first.
}
