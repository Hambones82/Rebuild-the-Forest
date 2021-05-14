using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void SelectBuildingDelegate(Building selectedBuilding);

public class UIManager : MonoBehaviour {
    public bool forcePlacement = false; 
    public GridMap gridMap;
    public MouseManager mouseManager;
    public BuildingManager buildingManager; 
    private GridTransform selectedGridTransform = null;
    public GridTransform SelectedGridTransform
    {
        get => selectedGridTransform;
    }
#pragma warning disable CS0414 // The field 'UIManager.commandPanelIsActive' is assigned but its value is never used
    private bool commandPanelIsActive = false;
#pragma warning restore CS0414 // The field 'UIManager.commandPanelIsActive' is assigned but its value is never used
    public Canvas UICanvas;

    private SelectBuildingDelegate selectBuildingDelegate;

    public UnityEvent OnSelectEvent;
    public UnityEvent OnDeselectEvent;

    [SerializeField]
#pragma warning disable CS0649 // Field 'UIManager.placementCursorPrefab' is never assigned to, and will always have its default value null
    private PlacementCursor placementCursorPrefab;
#pragma warning restore CS0649 // Field 'UIManager.placementCursorPrefab' is never assigned to, and will always have its default value null

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

    public void SelectGridTransform(GridTransform clickedGridTransform)
    {
        if(clickedGridTransform == null)
        {
            selectedGridTransform = null;
        }
        else
        {
            clickedGridTransform.GetComponent<MouseSelector>().Select(); 
            selectedGridTransform = clickedGridTransform; 
            OnSelectEvent.Invoke();
        }
    }
}




