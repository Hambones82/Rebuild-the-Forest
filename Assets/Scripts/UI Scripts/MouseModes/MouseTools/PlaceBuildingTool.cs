using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class, itself, might want the building cursor.  rather than ... what is it, ui manager?
public class PlaceBuildingTool : MouseTool
{
    private static BuildingManager buildingManager;

    private static PlaceBuildingTool _instance;
    public static PlaceBuildingTool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlaceBuildingTool();
                buildingManager = BuildingManager.Instance;
            }
            return _instance;
        }
    }

    private Building buildingToPlacePrefab;
    private ActorUnit actorUnit;


    //ok... big issue here is... we really want to determine the top left (?) or bottom left (?) position...  for the building
    public override bool LeftClick(Vector3 mousePosition, UIManager uiManager)
    {
        //Vector2Int mapCoords = uiManager.gridMap.WorldToMap(mousePosition);
        UnitActionController actionController = actorUnit.GetComponent<UnitActionController>();
        PlacementCursor cursor = uiManager.PlacementCursor;
        Vector2Int mapCoords = cursor.GetComponent<GridTransform>().topLeftPosMap;

        if (buildingManager.CanPlaceBuildingAt(buildingToPlacePrefab, mapCoords))
        {
            //set the unit action controll with two actions -- a move and a build.  define the build action.
            //generate move action, schedule it
            MoveAction moveAction = ObjectPool.Get<MoveAction>();
            moveAction.Initialize(actorUnit.gameObject);
            moveAction.SetMapDestination(mapCoords);
            actionController.DoAction(moveAction);
            //generate transform action, schedule it
            TransformIntoBuildingAction transformAction = ObjectPool.Get<TransformIntoBuildingAction>();
            transformAction.Initialize(actorUnit.gameObject, buildingToPlacePrefab);
            transformAction.worldBuildLocation = mousePosition;
            transformAction.mapBuildLocation = mapCoords;
            actionController.DoAction(transformAction);
        }
        
        return false;
    }

    public override bool RightClick(Vector3 mousePosition, UIManager uiManager)
    {
        return false;
    }
    
    
    public override void StartTool(UIManager uiManager, UnityEngine.Object buildingTypeObject, UnityEngine.Object actorUnitComponent)
    {
        BuildingType buildingType = (BuildingType)buildingTypeObject;
        buildingToPlacePrefab = buildingType.BuildingPrefab;
        
        ActorUnit actor = (ActorUnit)actorUnitComponent;
        this.actorUnit = actor;

        PlacementCursor cursor = uiManager.PlacementCursor;
        cursor.setBuilding(buildingToPlacePrefab);
        cursor.enableCursor();
    }

    public override void EndTool(UIManager uiManager)
    {
        uiManager.PlacementCursor.disableCursor();
    }

    public override void Cancel()
    {
        EndTool(UIManager.Instance);
        UIManager.Instance.SelectGridTransform(null);
    }
}
