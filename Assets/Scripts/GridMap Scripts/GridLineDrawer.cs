using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLineDrawer : MonoBehaviour {

    //public Sprite tileSprite;

    public bool useAttachedGrid = true;
    public Grid grid;

    public bool drawGridInEditor = true;
    public bool drawGridInGame = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        if (drawGridInEditor)
        {
            Gizmos.color = Color.gray;
            //this is hackish. we are assuming 2d game stuff is all at z = 0 and that the camera points perpendicular to the z=0 plane
            Vector3 cameraCenter = Camera.main.transform.position;
            cameraCenter.z = 0;
            //get left side of camera in world space
            float topSide = cameraCenter.y + Camera.main.orthographicSize;
            float leftSide = cameraCenter.x - (Camera.main.orthographicSize * Camera.main.aspect);
            float bottomSide = cameraCenter.y - Camera.main.orthographicSize;
            float rightSide = cameraCenter.x + (Camera.main.orthographicSize * Camera.main.aspect);
            Vector3Int topLeftCenterCell = grid.WorldToCell(new Vector3(leftSide, topSide, 0));
            Vector3Int bottomRightCenterCell = grid.WorldToCell(new Vector3(rightSide, bottomSide, 0));
            int leftCellNum = topLeftCenterCell.x;
            int rightCellNum = bottomRightCenterCell.x;
            int topCellNum = topLeftCenterCell.y;
            int bottomCellNum = bottomRightCenterCell.y;

            Vector3Int cellToDraw = new Vector3Int(0, 0, 0);
            Vector3 cellToDrawWorld = new Vector3(0, 0, 0);
            for (int i = leftCellNum; i <= rightCellNum; i++)
            {
                for (int j = bottomCellNum; j <= topCellNum; j++)
                {
                    cellToDraw.x = i;
                    cellToDraw.y = j;
                    cellToDrawWorld = grid.GetCellCenterWorld(cellToDraw);
                    
                    DrawCell(cellToDrawWorld);
                }
            }
            
        }
    }

    void DrawCell(Vector3 cellWorldPos)
    {
        Vector3 cellSize = grid.cellSize/2;
        GizmosExtensionMethods.DrawCameraParallelRectangle(cellWorldPos-cellSize, cellWorldPos + cellSize);
    }

    void OnValidate()
    {
        if(useAttachedGrid == true)
        {
            grid = this.gameObject.GetComponent<Grid>();
        }
        else
        {
            grid = null;
        }
    }
}
