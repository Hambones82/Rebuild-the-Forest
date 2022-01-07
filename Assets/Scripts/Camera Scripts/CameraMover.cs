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
    public float zoomSpeed;
    public float maxZoom;
    public float minZoom;
    
	void Start () {
        attachedCamera = GetComponent<Camera>();
        cameraTransform = GetComponent<Transform>();
        ConstrainToMap();
	}
	
    public void moveCamera(Vector2 direction)
    {
        Vector3 v3Direction = new Vector3(direction.x, direction.y);
        cameraTransform.position = cameraTransform.position + v3Direction * moveSpeed * Time.unscaledDeltaTime * attachedCamera.orthographicSize;
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
    public void zoomIn()
    {
        float rawNewCameraSize = attachedCamera.orthographicSize / zoomSpeed;
        float newCameraSize = Mathf.Clamp(rawNewCameraSize, minZoom, maxZoom);
        attachedCamera.orthographicSize = newCameraSize;
    }

    public void zoomOut()
    {
        float rawNewCameraSize = attachedCamera.orthographicSize * zoomSpeed;
        float newCameraSize = Mathf.Clamp(rawNewCameraSize, minZoom, maxZoom);
        attachedCamera.orthographicSize = newCameraSize; 
    }

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
