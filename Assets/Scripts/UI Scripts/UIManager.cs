using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public delegate void SelectBuildingDelegate(Building selectedBuilding);

public class UIManager : MonoBehaviour {
    public PlayerResourceData currentPlayer;//?
    public bool forcePlacement = false;
    public PlacementCursor cursorPrefab;
    public GridMap gridMap;
    public MouseManager mouseManager;
    public BuildingManager buildingManager; 
    private GridTransform selectedGridTransform = null;
    public GridTransform SelectedGridTransform
    {
        get => selectedGridTransform;
    }
    private PlacementCursor placementCursor;
    public CommandPanel commandPanelPrefab; //?
    private CommandPanel commandPanel;
    private bool commandPanelIsActive = false;
    public Canvas UICanvas;

    private SelectBuildingDelegate selectBuildingDelegate;

    public UnityEvent OnSelectEvent;
    public UnityEvent OnDeselectEvent;

    public bool placementCursorIsActive
    {
        get { return placementCursor.isActive; }
    }

    public enum MouseState
    {
        no_object_selected,
        object_selected,
        object_to_place, //look at this one.
        object_to_select
    }

    private MouseState mouseState = MouseState.no_object_selected;
    
    void Awake () {
        placementCursor = Instantiate(cursorPrefab, gridMap.GetComponent<Transform>());
	}
	
	// Update is called once per frame
	void Update () {
        //update location of cursor if it moves.
        if(placementCursorIsActive)
        {
            placementCursor.moveCursorTo(mouseManager.GetMouseWorldCoords());
        }
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
            case MouseState.object_to_place:
                //assert not is null 
                PlaceBuildingByCursor();
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
            case MouseState.object_to_place:
                EndPlacementByCursor();
                break;
            default:
                break;
        }
    }

    public void StartPlacementByCursor(BuildingType buildingType, PlayerResourceData playerData, bool forced = false)
    {
        StartPlacementByCursor(BuildingLoader.Instance.GetPrefab(buildingType).GetComponent<Building>(), playerData, forced);
    }

    //this function enables the cursor, and sets it to a building
     //new class...
    private void StartPlacementByCursor(Building building, PlayerResourceData playerData, bool forced)
    {
        this.currentPlayer = playerData;
        this.forcePlacement = forced;
        //maybe we would include a check to the building manager whether you can start -- e.g., if you have the resources or whatever...
        if (buildingManager.CanPlaceBuilding(building))
        {
            placementCursor.setBuilding(building);
            placementCursor.enableCursor();
            mouseState = MouseState.object_to_place;
            if(selectedGridTransform != null && commandPanelIsActive)
            {
                selectedGridTransform.GetComponent<MouseSelector>().DeSelect();
            }
        }
        else
        {
            //error message -- i guess we need to do a ui error system.
        }
    }

    private void EndPlacementByCursor()
    {
        placementCursor.disableCursor();
        mouseState = MouseState.no_object_selected;
    }

    public void PlaceBuildingByCursor()
    {
        Vector2Int mapCoordsDest = placementCursor.GetComponent<GridTransform>().topLeftPosMap;
        Building buildingToPlace = placementCursor.currentBuildingPrefab;
        if (buildingManager.CanPlaceBuildingAt(buildingToPlace, mapCoordsDest) &&
            currentPlayer.CanBuild(buildingToPlace))
        {
            Building building = buildingManager.SpawnBuildingAt(buildingToPlace, mapCoordsDest);
            //add player logic...
            currentPlayer.BuildBuilding(building);
            placementCursor.disableCursor();
            mouseState = MouseState.no_object_selected;
            building.GetComponent<CommandWindowInvoker>().SetUIManager(this);
        }
        else
        {
            //some error message
        }
    }
    //--different class -- maybe building placement class

    //below -- in a different class probably.
    public void OpenBuildingPanel(Building building)
    {
        CommandPanel cPanel = Instantiate(building.GetComponent<CommandWindowInvoker>().commandPanelPrefab, UICanvas.GetComponent<Transform>());
        cPanel.SetUIManager(this);
        cPanel.SetBuilding(building);
        commandPanel = cPanel;
        
        commandPanelIsActive = true;
    }

    public void CloseBuildingPanel(Building building)
    {
        Destroy(commandPanel.gameObject);
        commandPanelIsActive = false;
    }

    public void StartSelectBuilding(SelectBuildingDelegate sbd)
    {
        selectBuildingDelegate = sbd;
        mouseState = MouseState.object_to_select;
        //maybe fix the state machine.  this ad hoc changes are bad... want a state machine that has defined transitions...
    }
     
}




