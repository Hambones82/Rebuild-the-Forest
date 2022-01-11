using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void SelectBuildingDelegate(Building selectedBuilding);

[DefaultExecutionOrder(-5)]
public class UIManager : MonoBehaviour {
    public CameraMover cameraMover;
    public bool forcePlacement = false; 
    public GridMap gridMap;
    public MouseManager mouseManager;
    public BuildingManager buildingManager; 
    private GridTransform selectedGridTransform = null;
    public GridTransform SelectedGridTransform
    {
        get => selectedGridTransform;
    }

    private bool commandPanelIsActive = false;

    public Canvas UICanvas;

    private SelectBuildingDelegate selectBuildingDelegate;

    public UnityEvent OnSelectEvent;
    public UnityEvent OnDeselectEvent;

    [SerializeField]
    private PlacementCursor placementCursorPrefab;

    private PlacementCursor placementCursor;
    public PlacementCursor PlacementCursor
    {
        get
        {
            if(placementCursor == null)
            {
                placementCursor = Instantiate(placementCursorPrefab, gridMap.transform);
            }
            return placementCursor;
        }

    }
    
    private MouseTool currentMouseTool = BaseTool.Instance;

    private static UIManager _instance;
    public static UIManager Instance { get => _instance; }

    private void Awake()
    {
        if(_instance != null)
        {
            throw new InvalidOperationException("Trying to instantiate two UIManagers, but it is a singleton.");
        }
        else
        {
            _instance = this;
        }
    }

    public void LeftClickAt(Vector3 mouseWorldPosition)
    {
        if(currentMouseTool.LeftClick(mouseWorldPosition, this) == false)
        {
            SwitchMouseTool(BaseTool.Instance);
        }
    }

    public void rightClickAt(Vector3 mouseWorldPosition)
    {
        if(currentMouseTool.RightClick(mouseWorldPosition, this) == false)
        {
            SwitchMouseTool(BaseTool.Instance);
        }
    }
    
    public void SwitchMouseTool(MouseTool mouseTool, UnityEngine.Object unityObject1 = null, UnityEngine.Object unityObject2 = null)//provide an optional game object?  i think that would be a good idea.
    {
        currentMouseTool.EndTool(this);
        currentMouseTool = mouseTool;
        currentMouseTool.StartTool(this, unityObject1, unityObject2);
    }

    public void HardSelectGridTransform(GridTransform clickedGridTransform)
    {
        if(clickedGridTransform == null || clickedGridTransform == selectedGridTransform)
        {
            return;
        }
        SwitchMouseTool(BaseTool.Instance);
        LeftClickAt(clickedGridTransform.transform.position);
    }

    public void SelectGridTransform(GridTransform clickedGridTransform)
    {
        if(clickedGridTransform == null || clickedGridTransform.GetComponent<MouseSelector>() == null)
        {
            selectedGridTransform = null;
            OnDeselectEvent.Invoke();
        }
        else
        {
            if(selectedGridTransform!=null)
            {
                OnDeselectEvent.Invoke();
            }
            clickedGridTransform.GetComponent<MouseSelector>().Select(); 
            selectedGridTransform = clickedGridTransform; 
            OnSelectEvent.Invoke();
        }
    }

    
    public void DeselectMouseSelector(MouseSelector objectToDisable)
    {
        if(selectedGridTransform != null && selectedGridTransform == objectToDisable.GetComponent<GridTransform>())
        {
            currentMouseTool.Cancel();
            currentMouseTool = BaseTool.Instance;
        }
    }

    public void ZoomIn()
    {
        cameraMover.zoomIn();
    }

    public void ZoomOut()
    {
        cameraMover.zoomOut();
    }
}




