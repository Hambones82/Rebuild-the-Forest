using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this gets attached to a camera and lets it be moved via unity events by exposing moveCameraXX functions

public class CameraMover : MonoBehaviour, IGameManager
{

    [SerializeField] private CameraConfigData _configData;
    private GridMap _gridMap; //the grid map that the camera is moving across
    private Transform cameraTransform; // transform of the camera
    private Camera attachedCamera; //the attached camera


    private ServiceLocator _serviceLocator;
    public void SelfInit(ServiceLocator serviceLocator)
    {
        _serviceLocator = serviceLocator;
        serviceLocator.RegisterService<CameraMover>(this);
    }

    public void MutualInit()
    {
        attachedCamera = Camera.main;
        cameraTransform = attachedCamera.transform;
        _gridMap = _serviceLocator.LocateService<GridMap>();
        ConstrainToMap();
    }
	
    public void moveCamera(Vector2 direction)
    {
        Vector3 v3Direction = new Vector3(direction.x, direction.y);
        cameraTransform.position = cameraTransform.position + v3Direction * _configData.moveSpeed * Time.unscaledDeltaTime * attachedCamera.orthographicSize;
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
        float rawNewCameraSize = attachedCamera.orthographicSize / _configData.zoomSpeed;
        float newCameraSize = Mathf.Clamp(rawNewCameraSize, _configData.minZoom, _configData.maxZoom);
        attachedCamera.orthographicSize = newCameraSize;
    }

    public void zoomOut()
    {
        float rawNewCameraSize = attachedCamera.orthographicSize * _configData.zoomSpeed;
        float newCameraSize = Mathf.Clamp(rawNewCameraSize, _configData.minZoom, _configData.maxZoom);
        attachedCamera.orthographicSize = newCameraSize; 
    }

    public void ConstrainToMap()
    {
        Vector3 newPosition = new Vector3(attachedCamera.transform.position.x, attachedCamera.transform.position.y, attachedCamera.transform.position.z);
        Rect mapExtents = _gridMap.GetCellCenterWorldRect();
        if(mapExtents.size.x <= _configData.padding *2)
        {
            newPosition.x = mapExtents.xMin + mapExtents.width / 2;
        }
        else
        {
            newPosition.x = Mathf.Clamp(newPosition.x, mapExtents.xMin + _configData.padding, mapExtents.xMax - _configData.padding);
        }
        if(mapExtents.size.y <= _configData.padding * 2)
        {
            newPosition.y = mapExtents.yMin + mapExtents.height / 2;
        }
        else
        {
            newPosition.y = Mathf.Clamp(newPosition.y, mapExtents.yMin + _configData.padding, mapExtents.yMax - _configData.padding);
        }
        attachedCamera.transform.position = newPosition;
    }

}
