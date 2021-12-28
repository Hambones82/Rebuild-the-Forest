using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinderTester : MonoBehaviour
{
    public GridMap gridMap;

    bool during = false;
    Vector2Int startPos;
    Vector2Int endPos;

    List<Vector2Int> lastPath;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(!during)
            {
                startPos = MouseManager.Instance.GetMouseMapCoords();
                during = true;
            }
            else
            {
                endPos = MouseManager.Instance.GetMouseMapCoords();
                during = false;
                PathFinder.GetPath(startPos, endPos, out lastPath);
                Debug.Log($"Start pos: {startPos.ToString()}");
                Debug.Log($"End pos: {endPos.ToString()}");
                if (lastPath == null) Debug.Log("no path");
                else
                {
                    for (int i = 0; i < lastPath.Count; i++)
                    {
                        Debug.Log($"step {i}: {lastPath[i].ToString()}");
                    }
                }
                
            }
        }
    }

    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1);
        foreach(Vector2Int pathCell in lastPath)
        {
            Gizmos.DrawCube(gridMap.MapToWorld(pathCell), gridMap.mapCellToCellDistance);
        }
    }
    */
}
