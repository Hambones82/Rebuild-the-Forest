using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this gets attached to a camera and lets it be moved via unity events by exposing moveCameraXX functions

public class CameraMover : MonoBehaviour {

    public float padding; // number of world units that the center pads up to when reaches the border
    public float moveSpeed; //how fast the camera moves
    public GridMap gridMap; //the grid map that the camera is moving across
    Transform cameraTransform; // transform of the camera
    Camera attachedCamera; //the attached camera

	// Use this for initialization
	void Start () {
        attachedCamera = GetComponent<Camera>();
        cameraTransform = GetComponent<Transform>();
        ConstrainToMap();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void moveCamera(Vector2 direction)
    {
        Vector3 v3Direction = new Vector3(direction.x, direction.y);
        cameraTransform.position = cameraTransform.position + v3Direction * moveSpeed * Time.deltaTime;
        ConstrainToMap();

    }

    public void moveCameraTo(Vector3 positionTo)
    {
        cameraTransform.position = new Vector3(positionTo.x, positionTo.y, cameraTransform.position.z);
        ConstrainToMap();
    }

    public void moveCameraUp() { moveCamera(Vector2.up); }
    public void moveCameraDown() { moveCamera(Vector2.down); }
    public void moveCameraLeft() { moveCamera(Vector2.left); }
    public void moveCameraRight() { moveCamera(Vector2.right); }

    public void ConstrainToMap()
    {
        Vector3 newPosition = new Vector3(attachedCamera.transform.position.x, attachedCamera.transform.position.y, attachedCamera.transform.position.z);
        Rect mapExtents = gridMap.GetCellCenterWorldRect();
        if(mapExtents.size.x <= padding*2)
        {
            newPosition.x = mapExtents.xMin + mapExtents.width / 2;
        }
        else
        {
            newPosition.x = Mathf.Clamp(newPosition.x, mapExtents.xMin + padding, mapExtents.xMax - padding);
        }
        if(mapExtents.size.y <= padding * 2)
        {
            newPosition.y = mapExtents.yMin + mapExtents.height / 2;
        }
        else
        {
            newPosition.y = Mathf.Clamp(newPosition.y, mapExtents.yMin + padding, mapExtents.yMax - padding);
        }
        attachedCamera.transform.position = newPosition;
    }
}
