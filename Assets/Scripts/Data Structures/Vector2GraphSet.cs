using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


//let's redo this whole thing with a row-by-row technique that we optimize with a hierarchical space partition

public class Vector2GraphSet 
{   
    /*
    private class Node
    {
        public Vector2Int position;
        public int id;
        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }

    private class ConnectedComponent
    {
        public HashSet<Vector2Int> nodes;
        public int id;
    }
    */    

    private int[,] nodes; //shadows the dictionary - used for fast lookups
                          //stores the id of the connected component
    Dictionary<int, List<Vector2Int>> connectedComponents;

    private int width, height;
    UniqueIDProvider uniqueIDProvider;
    

    //also need a returnID function

    public Vector2GraphSet(int width, int height)
    {
        this.width = width;
        this.height = height;
        nodes = new int[width, height];
        //initialize nodes to -1
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                nodes[i, j] = -1;        
        connectedComponents = new Dictionary<int, List<Vector2Int>>();        
        uniqueIDProvider = new UniqueIDProvider();
    }

    private void AddPositionToID(int id, Vector2Int position)
    {
        nodes[position.x, position.y] = id;
        connectedComponents[id].Add(position);
    }

    //problem is vector2int is a class, should be a struct...
    private void RemovePositionFromID(int id, Vector2Int position)
    {
        nodes[position.x, position.y] = -1;
        /*
        foreach(Vector2Int pos in connectedComponents[id])
        {
            Console.Write($"component contents of id {id}: {pos}");
        }*/
        //Console.Write("\n");
        if(!connectedComponents[id].Remove(position))
        {
            throw new InvalidOperationException($"attempting to remove not-present position {position} from id {id}");
        }
    }

    private int GetNewID()
    {
        int newID = uniqueIDProvider.GetNewID();
        if (!connectedComponents.ContainsKey(newID))
        {
            connectedComponents.Add(newID, new List<Vector2Int>());
        }
        return newID;
    }

    public void AddPosition(Vector2Int position)
    {
        //if outside, return?
        if(!position.IsInBounds(width, height))
        {
            throw new ArgumentOutOfRangeException($"{position} is not in range 0-{width}, 0-{height}");
        }
        //get all neighbors that are in bounds
        List<Vector2Int> candidateNeighbors = position.GetNeighborsInBounds(width, height);
        List<Vector2Int> neighbors = new List<Vector2Int>();
        //Console.WriteLine($"adding position {position}");
        foreach(Vector2Int neighbor in candidateNeighbors)
        {
            //Console.WriteLine(neighbor);
            if (nodes[neighbor.x, neighbor.y] != -1)
            {
                neighbors.Add(neighbor);
            }
        }
        if(neighbors.Count == 0)
        {
            //add a new connected component
            int newID = GetNewID();
            AddPositionToID(newID, position);
            
            return;
        }
        if(neighbors.Count == 1)
        {
            //add to the connected component of the neighbor
            Vector2Int neighbor = neighbors[0];
            int idToAdd = nodes[neighbor.x, neighbor.y];
            AddPositionToID(idToAdd, position);
            return;
        }
        //get the connected components of the neighbors
        HashSet<int> neighborComponentIDs = new HashSet<int>();
        foreach(Vector2Int neighbor in neighbors)
        {
            int neighborID = nodes[neighbor.x, neighbor.y];
            if(neighborID != -1)
            {
                neighborComponentIDs.Add(neighborID);
            }
        }
        int idToJoinTo = neighborComponentIDs.Min();
        neighborComponentIDs.Remove(idToJoinTo);
        //ok i think the above is fine -- join to the lowest numbered neighbor.  but...
        //we have to make sure to 
        AddPositionToID(idToJoinTo, position);

        //perform union
        foreach(int idToJoin in neighborComponentIDs)
        {
            //Console.WriteLine($"joining {idToJoin} to {idToJoinTo}");
            JoinComponents(idToJoinTo, idToJoin);
        }
    }

    private void JoinComponents(int baseID, int idToJoin)
    {
        List<Vector2Int> positionsToJoin = new List<Vector2Int>(connectedComponents[idToJoin]);
        foreach(Vector2Int positionToJoin in positionsToJoin)
        {
            //Console.WriteLine($"adding position {positionToJoin}");
            //Console.WriteLine($"baseID: {baseID}; idToJoin: {idToJoin}");            
            //maybe an optimization is to have all nodes have the id of the connected component they belong to
            //then we can just change the id of the connected component when they are joined...?
            //but then would breaking them off be messy...?  maybe yes but it might still be faster?
            AddPositionToID(baseID, positionToJoin);            
        }
        ReturnConnectedComponent(idToJoin);
    }

    private void ReturnConnectedComponent(int id)
    {
        connectedComponents[id].Clear();
        uniqueIDProvider.ReturnID(id);
        //Console.WriteLine($"returned component {id}");
    }
    //need remove position...
    //removing requires potentially splitting and thus getting a new ID...

    //given a list of nodes to test for connectivity, keep in list if tey connect.  if they dno't connect, move them all to a 
    //subsequent list, then repeat for that list.
    //the result is all of the connected components.  for the first list, we keep as is, and create new sets (?) for the rest

    //NEEDS TO BE FIXED - i think we are getting an error where there is a circular traversal of the graph...
    public void RemovePosition(Vector2Int position)
    {
        if (!position.IsInBounds(width, height))
        {
            throw new ArgumentOutOfRangeException($"{position} is not in range 0-{width}, 0-{height}");
        }
        int idOfRemoved = nodes[position.x, position.y];
        //Console.WriteLine($"num entries in {idOfRemoved}: {connectedComponents[idOfRemoved].Count()}");
        RemovePositionFromID(idOfRemoved, position);
        //Console.WriteLine($"num entries in {idOfRemoved}: {connectedComponents[idOfRemoved].Count()}");
        if (connectedComponents[idOfRemoved].Count == 0)
        {
            ReturnConnectedComponent(idOfRemoved);
            return;
        }
        //
        List<Vector2Int> initialNeighbors = new List<Vector2Int>();
        foreach(Vector2Int pos in position.GetNeighborsInBounds(width, height))
        {
            if (nodes[pos.x, pos.y] != -1) { initialNeighbors.Add(pos); }
        }
            

        if (initialNeighbors.Count == 1) return;
        List<List<Vector2Int>> splitComponents = new List<List<Vector2Int>>();

        bool[,] visited = new bool[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++) 
                visited[x, y] = false;
        while(initialNeighbors.Count > 0)
        {            
            Queue<Vector2Int> positionsToVisit = new Queue<Vector2Int>();
            Vector2Int currentNeighbor = initialNeighbors[0];
            initialNeighbors.RemoveAt(0);
            if (visited[currentNeighbor.x, currentNeighbor.y]) continue;
            positionsToVisit.Enqueue(currentNeighbor);           
            List<Vector2Int> componentUnderConstruction = new List<Vector2Int>();
            componentUnderConstruction.Add(currentNeighbor);
            while(positionsToVisit.Count > 0)
            {                
                Vector2Int positionToVisit = positionsToVisit.Dequeue();
                List<Vector2Int> candidateNeighborsToEnqueue = positionToVisit.GetNeighborsInBounds(width, height);                
                foreach (Vector2Int candidateNeighbor in candidateNeighborsToEnqueue)
                {
                    if (!visited[candidateNeighbor.x, candidateNeighbor.y]
                        && nodes[candidateNeighbor.x, candidateNeighbor.y] != -1)
                    {
                        positionsToVisit.Enqueue(candidateNeighbor);
                        visited[candidateNeighbor.x, candidateNeighbor.y] = true;
                        componentUnderConstruction.Add(candidateNeighbor);
                    }                        
                }
                
            }
            splitComponents.Add(componentUnderConstruction);
                    
        }
        //for each split component, create new components
        splitComponents.RemoveAt(0);
        foreach (List<Vector2Int> component in splitComponents)
        {
            int newID = GetNewID();
            foreach (Vector2Int positionToAdd in component)
            {
                AddPositionToID(newID, positionToAdd);
            }
        }
    }


    public int GetGraphID(Vector2Int position)
    {
        return nodes[position.x, position.y];
    }

    public override string ToString()
    {
        string retval = "    ";
        for(int j = 0; j < width; j++)
        {
            retval += $" {j.ToString("D2")} ";
        }
        retval += "\n";
        for(int i = 0; i < height; i++)
        {
            retval += $" {i.ToString("D2")} ";
            for(int j = 0; j < width; j++)
            {
                int numToAdd = nodes[j, i];
                if (numToAdd < 0) retval += "  x";
                else retval += " " + nodes[j, i].ToString("D2");
                retval += " ";
            }
            retval += "\n";
        }
        /*
        retval += "Connected components: \n";
        foreach(KeyValuePair<int, List<Vector2Int>> componentPositions in connectedComponents)
        {
            retval += $"component {componentPositions.Key}: ";
            foreach(Vector2Int position in componentPositions.Value)
            {
                retval += position + " ";
            }
            retval += "\n";
        }
        */
        return retval;
    }    
}
