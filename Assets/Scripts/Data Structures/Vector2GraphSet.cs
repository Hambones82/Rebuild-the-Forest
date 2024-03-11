using PlasticPipe.PlasticProtocol.Messages;
using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2GraphSet 
{   
    private struct Node
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

    private int[,] nodes; //shadows the dictionary - used for fast lookups
                          //stores the id of the connected component
    Dictionary<int, List<Vector2Int>> connectedComponents;

    private int width, height;

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
    }

    private void AddPositionToID(int id, Vector2Int position)
    {
        nodes[position.x, position.y] = id;
        connectedComponents[id].Add(position);        
    }

    private void RemovePositionFromID(int id, Vector2Int position)
    {
        nodes[position.x, position.y] = -1;
        connectedComponents[id].Remove(position);
    }

    public void AddPosition(Vector2Int position)
    {
        //get all neighbors that are in bounds
        List<Vector2Int> neighbors = position.GetNeighborsInBounds(width, height);
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
        int idToJoinTo = nodes[neighbors[0].x, neighbors[0].y];
        AddPositionToID(idToJoinTo, position);

        //perform union
        for(int idToJoin = 1; idToJoin < neighborComponentIDs.Count; idToJoin++)
        {
            JoinComponents(idToJoinTo, idToJoin);
        }
    }

    private void JoinComponents(int baseID, int idToJoin)
    {
        foreach(Vector2Int positionToJoin in connectedComponents[idToJoin])
        {
            //maybe an optimization is to have all nodes have the id of the connected component they belong to
            //then we can just change the id of the connected component when they are joined...?
            //but then would breaking them off be messy...?  maybe yes but it might still be faster?
            AddPositionToID(baseID, positionToJoin);
            RemovePositionFromID(idToJoin, positionToJoin);
        }
    }

    
    public void RemovePosition(Vector2Int position)
    {
        int idOfRemoved = nodes[position.x, position.y];
        nodes[position.x, position.y] = -1;
        
        //find the combos that are the same
        //we need to just do the search.  the current neighbors are all part of the same network
        //using Direction so we can indicate which neighbor we are coming from
        int[,] visited = new int[width, height];
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                visited[i, j] = -1;
        
        
        Stack<Vector2Int> positionsUnderExamination = new Stack<Vector2Int>();
        //put the first four into positionsunderexamination.  also, set visited for each one, based on direction
        Vector2Int left = position + Vector2Int.left;
        Vector2Int right = position + Vector2Int.right;
        Vector2Int up = position + Vector2Int.up;
        Vector2Int down = position + Vector2Int.down;

        List<int>[] connectionsToCheck = new List<int>[4];
        bool[,] confirmedConnections = new bool[4,4];
        //initialize to false
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                confirmedConnections[i,j] = false;
            }
        }
        List<Vector2Int>[] components = new List<Vector2Int>[4];
        List<Vector2Int>[] continuations = new List<Vector2Int>[4];
        //want to put the items traversed into the components as we traverse, but also record "continuations"
        //so that when done with traversal, we can finish up the components that need to be split off

        for(int i = 0; i < 4; i++)
        {
            connectionsToCheck[i] = new List<int>();
            connectionsToCheck[i].Add(0);
            connectionsToCheck[i].Add(1);
            connectionsToCheck[i].Add(2);
            connectionsToCheck[i].Add(3);
            connectionsToCheck[i].Remove(i);
        }

        if (left.x - 1 >= 0 && nodes[left.x, left.y] != -1)
        {
            positionsUnderExamination.Push(left);
            visited[position.x - 1, position.y] = 0;
            components[0].Add(left);
        }
        else
        {
            connectionsToCheck[0].Clear();
            connectionsToCheck[1].Remove(0);
            connectionsToCheck[2].Remove(0);
            connectionsToCheck[3].Remove(0);
        }
        if(right.x + 1 < width && nodes[right.x, right.y] != -1)
        {
            positionsUnderExamination.Push(right);
            visited[position.x + 1, position.y] = 1;
            components[1].Add(right);
        }
        else
        {
            connectionsToCheck[1].Clear();
            connectionsToCheck[0].Remove(1);
            connectionsToCheck[2].Remove(1);
            connectionsToCheck[3].Remove(1);
        }
        if(up.y + 1 < height && nodes[up.x, up.y] != -1)
        {
            positionsUnderExamination.Push(up);
            visited[position.x, position.y + 1] = 2;
            components[2].Add(up);
        }
        else
        {
            connectionsToCheck[2].Clear();
            connectionsToCheck[0].Remove(2);
            connectionsToCheck[1].Remove(2);
            connectionsToCheck[3].Remove(2);
        }
        if(down.y - 1 >= 0 && nodes[down.x, down.y] != -1)
        {
            positionsUnderExamination.Push(down);
            visited[position.x, position.y - 1] = 3;
            components[3].Add(down);
        }
        else
        {
            connectionsToCheck[3].Clear();
            connectionsToCheck[0].Remove(3);
            connectionsToCheck[1].Remove(3);
            connectionsToCheck[2].Remove(3);
        }



        while (positionsUnderExamination.Count > 0)
        {
            Vector2Int current = positionsUnderExamination.Pop();
            int currentVisited = visited[current.x, current.y];
            if (connectionsToCheck[currentVisited].Count == 0)
            {
                continuations[currentVisited].Add(current);
                continue;
            }
            foreach(Vector2Int neighbor in current.GetNeighborsInBounds(width, height))
            {
                if (nodes[neighbor.x, neighbor.y] == -1)
                {
                    continue;
                }
                int neighborVisited = visited[neighbor.x, neighbor.y];
                if (neighborVisited == -1)
                {
                    visited[neighbor.x, neighbor.y] = currentVisited;
                    positionsUnderExamination.Push(neighbor);
                    components[currentVisited].Add(neighbor);
                }
                else
                {
                    if (connectionsToCheck[currentVisited].Contains(neighborVisited))
                    {
                        connectionsToCheck[currentVisited].Remove(neighborVisited);
                        connectionsToCheck[neighborVisited].Remove(currentVisited);
                    }
                    //set the confirmed connections
                    confirmedConnections[currentVisited, neighborVisited] = true;
                    confirmedConnections[neighborVisited, currentVisited] = true;

                }
            }

            //consolidate the confirmed connections
            for(int i = 0; i < 4; i++)
            {
                for(int j = i+1; j < 4; j++)
                {
                    if (confirmedConnections[i,j])
                    {
                        for(int k = j+1; k < 4; k++)
                        {
                            if (confirmedConnections[j,k])
                            {
                                confirmedConnections[i, k] = true;
                                confirmedConnections[k, i] = true;
                            }                                
                        }
                    }
                }
            }
            //above: problem is that...  we havent' tracked which directions aren't even part of the search
            //we could check connectionsToCheck to see if it's null because it's only initialized when there's a valid neighbor
            //of the initial node
            //if there are any falses, that represents a component that needs to be split off
            

            //i think we should scrap all this and do a hierarchical search with a row-by-row analysis



            
                        
        }


    }

    //get ID?
    public int GetGraphID(Vector2Int position)
    {
        return nodes[position.x, position.y];
    }
}
