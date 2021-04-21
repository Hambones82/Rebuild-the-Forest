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
    private bool commandPanelIsActive = false;
    public Canvas UICanvas;

    private SelectBuildingDelegate selectBuildingDelegate;

    public UnityEvent OnSelectEvent;
    public UnityEvent OnDeselectEvent;
    

    public enum MouseState
    {
        no_object_selected,
        object_selected,
        object_to_place, //look at this one.
        object_to_select
    }

    private MouseState mouseState = MouseState.no_object_selected;
    
    void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    
    public void clickAt(Vector3 mouseWorldPosition)
    {
        GridTransform clickedGridTransform = (GridTransform)gridMap.GetClosestClickedObject(mouseWorldPosition);
        switch (mouseState)
        {
            case MouseState.object_selected:
                if (clickedGridTransform == selectedGridTransform)
                {
                    break;
                }
                else // either you click on another building or on nothing.
                {
                    selectedGridTransform.GetComponent<MouseSelector>()?.DeSelect(); // therefore, you deselect the current selected one
                    OnDeselectEvent.Invoke();
                    //here we use null as a valid value... maybe we should use something else?
                    if (clickedGridTransform != null)
                    {
                        SelectGridTransform(clickedGridTransform);
                    }
                    else // clicked is null
                    {
                        selectedGridTransform = null; // set to null because nothing is selected
                        mouseState = MouseState.no_object_selected; //mouse state is that nothing is selected
                    }
                }
                break;
            case MouseState.no_object_selected:
                if (clickedGridTransform != null) //if you click on something
                {
                    SelectGridTransform(clickedGridTransform);
                    mouseState = MouseState.object_selected; //mouse state is that something is selected
                }
                break;
            case MouseState.object_to_select:
                if(clickedGridTransform == null)
                {
                    //i don't think nothing is the correct behavior...
                    //but anyway i'm trying to make a prototype so... i think we should focus on something more important
                    break;
                }
                Building building = clickedGridTransform.GetComponent<Building>();
                //Debug.Log("selecting for connection");
                if (building == selectedGridTransform.GetComponent<Building>()) Debug.Log("buildings are equal");
                if (building != null && selectBuildingDelegate != null)
                {
                    selectBuildingDelegate(building);
                    mouseState = MouseState.no_object_selected;
                    selectBuildingDelegate = null;
                    MouseSelector ms = selectedGridTransform.GetComponent<MouseSelector>();
                    if (ms != null)
                    {
                        ms.DeSelect();
                    }
                }
                if (selectedGridTransform != null) selectedGridTransform = null;
                break;
        }

    }

    private void SelectGridTransform(GridTransform clickedGridTransform)
    {
        clickedGridTransform.GetComponent<MouseSelector>().Select(); //select it
        selectedGridTransform = clickedGridTransform; //set it to the selected thing
        OnSelectEvent.Invoke();
    }

    public void rightClickAt(Vector3 vector3)
    {
        Vector2Int mapCoords = gridMap.WorldToMap(vector3);
        switch (mouseState)
        {
            case MouseState.no_object_selected:
                break;
            case MouseState.object_selected:
                ContextClickComponent contextClickComponent = selectedGridTransform.GetComponent<ContextClickComponent>();
                contextClickComponent?.DoContextClick(mapCoords);
                break;
            default:
                break;
        }
    }
    
}




