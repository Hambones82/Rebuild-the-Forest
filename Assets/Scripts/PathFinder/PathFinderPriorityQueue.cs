using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class PathFinderPriorityQueue
{
    private Node[,] nodes;
    private int _width;
    private int _height;

    private struct Node
    {
        public int priority;
        public Vector2Int lower;
        public Vector2Int higher;
        public Node(int inP, Vector2Int inL, Vector2Int inH)
        {
            priority = inP;
            lower = inL;
            higher = inH;
        }
        public override string ToString()
        {
            return $"priority: {priority}, lower: {lower}, higher: {higher}\n";
        }
        public void Clear()
        {
            priority = noPriority;
            lower = noPosition;
            higher = noPosition;
        }
    }
    public static readonly int noPriority = -1;
    public static readonly Vector2Int noPosition = new Vector2Int(-1, -1);
    private static readonly Node noNode = new Node(noPriority, noPosition, noPosition);

    private int numNodes = 0;
    private Vector2Int lowest = new Vector2Int(0,0);
    public PathFinderPriorityQueue(int width, int height)
    {
        nodes = new Node[width, height];
        _width = width;
        _height = height;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                nodes[x, y].priority = noPriority;
                nodes[x, y].lower = noPosition;
                nodes[x, y].higher = noPosition;
            }
        }
        numNodes = 0;
    }

    public int Count { get => numNodes; }

    public void Clear()
    {
        //Debug.Log($"Numnodes before: {numNodes}");
        Vector2Int current = DeQueue();
        while(current != noPosition)
        {
            current = DeQueue();
        }
        //Debug.Log($"Numnodes after: {numNodes}");
        /*
        for(int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                nodes[x,y].priority = noPriority;
                nodes[x, y].lower = noPosition;
                nodes[x, y].higher = noPosition;
            }
        }
        numNodes = 0;
        */
    }

    public bool Contains(Vector2Int pos)
    {
        return nodes[pos.x, pos.y].priority != noPriority;
    }

    public void Enqueue(int x, int y, int priority) //use a hint node as the starting place; hint must always be less than?
    {
        if(numNodes == 0)
        {
            lowest.x = x;
            lowest.y = y;
            nodes[x, y].priority = priority;
            nodes[x, y].lower = noPosition;
            nodes[x, y].higher = noPosition;
            numNodes++;
            return;
        }
        else
        {
            Vector2Int next = lowest;//the one we are going behind
            Vector2Int current = noPosition;//the one we are going after
            //find insertion point -- could improve speed, as this is O(n)
            while (priority > nodes[next.x,next.y].priority)
            {
                current = next;
                next = nodes[current.x, current.y].higher;
                if (next == noPosition) break;
            }
            if(current == noPosition)//lowest is first
            {
                nodes[lowest.x,lowest.y].lower = new Vector2Int(x, y);
                nodes[x, y].higher = lowest;
                nodes[x, y].priority = priority;
                lowest = new Vector2Int(x, y);
            }
            else if(next == noPosition)//current is last
            {
                nodes[current.x, current.y].higher = new Vector2Int(x, y);
                nodes[x, y].lower = current;
                nodes[x, y].priority = priority;
            }
            else //current is in the middle
            {
                nodes[current.x, current.y].higher = new Vector2Int(x, y);
                nodes[next.x, next.y].lower = new Vector2Int(x, y);
                nodes[x, y].priority = priority;
                nodes[x, y].lower = current;
                nodes[x, y].higher = next;
            }
                numNodes++;
        }
        
    }

    public Vector2Int DeQueue()
    {
        Vector2Int retVal;
        if(numNodes == 0)
        {
            return noPosition;
        }
        else if(numNodes == 1)
        {
            retVal = lowest;
            lowest = noPosition;
            numNodes = 0;
        }
        else
        {
            retVal = lowest;
            lowest = nodes[lowest.x, lowest.y].higher;
            nodes[lowest.x, lowest.y].lower = noPosition;
            numNodes--;
        }
        nodes[retVal.x, retVal.y].priority = noPriority;
        nodes[retVal.x, retVal.y].lower = noPosition;
        nodes[retVal.x, retVal.y].higher = noPosition;
        return retVal;
    }

    public void UpdatePriority(Vector2Int position, int newPriority)
    {
        if(Contains(position))
        {
            //Debug.Log("changing priority");
            ref Node currentNode = ref nodes[position.x, position.y];
            if(currentNode.lower != noPosition)
            {
                ref Node lowerNode = ref nodes[currentNode.lower.x, currentNode.lower.y];
                lowerNode.higher = currentNode.higher;
            }
            if(currentNode.higher != noPosition)
            {
                ref Node higherNode = ref nodes[currentNode.higher.x, currentNode.higher.y];
                higherNode.lower = currentNode.lower;
            }
            numNodes--;
            currentNode.Clear();
            Enqueue(position.x, position.y, newPriority);
        }
    }
    
    public override string ToString()
    {
        string retVal = $"number of nodes: {numNodes}\n";
        Vector2Int current = lowest;
        while(current != noPosition)
        {
            Node currentNode = nodes[current.x, current.y];
            retVal += $"tile {current.x}, {current.y}: {currentNode}\n";
            current = currentNode.higher;
        }
        return retVal;
    }
    


}