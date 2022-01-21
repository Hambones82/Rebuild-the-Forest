using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//need to initialize this as a parent of the gridmap.
public class PlacementCursor : Cursor
{
    private Building _currentBuildingPrefab;
    public Building currentBuildingPrefab
    {
        get { return _currentBuildingPrefab; }
    }

    public void setBuilding(Building building)
    {
        SpriteRenderer buildingSprite = building.GetComponent<SpriteRenderer>();
        GridTransform buildingGridTransform = building.GetComponent<GridTransform>();
        SpriteRenderer cursorSprite = GetComponent<SpriteRenderer>();
        GridTransform cursorGridTransform = GetComponent<GridTransform>();

        cursorSprite.sprite = buildingSprite.sprite;

        cursorSprite.color = buildingSprite.color;
        SetCursorAlpha();
        cursorGridTransform.SetSize(buildingGridTransform);

        _currentBuildingPrefab = building;
    }
}
