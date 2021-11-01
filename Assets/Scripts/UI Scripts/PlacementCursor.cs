using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//need to initialize this as a parent of the gridmap.
public class PlacementCursor : MonoBehaviour
{

    //at some point, need to assert that all dependencies exist

    public float cursorAlpha = .5f;

    private Building _currentBuildingPrefab;
    public Building currentBuildingPrefab
    {
        get { return _currentBuildingPrefab; }
    }
    
    private bool _isActive = false;
    public bool isActive
    {
        get { return _isActive; }
    }

    private void Update()
    {
        if(_isActive)
        {
            moveCursorTo(MouseManager.Instance.MouseWorldPosition);
        }
    }

    public void setBuilding(Building building)
    {
        SpriteRenderer buildingSprite = building.GetComponent<SpriteRenderer>();
        GridTransform buildingGridTransform = building.GetComponent<GridTransform>();
        SpriteRenderer cursorSprite = GetComponent<SpriteRenderer>();
        GridTransform cursorGridTransform = GetComponent<GridTransform>();
        Assert.IsNotNull(buildingSprite);
        Assert.IsNotNull(buildingGridTransform);
        Assert.IsNotNull(cursorSprite);
        Assert.IsNotNull(cursorGridTransform);

        cursorSprite.sprite = buildingSprite.sprite;

        cursorSprite.color = buildingSprite.color;
        SetCursorAlpha();
        cursorGridTransform.SetSize(buildingGridTransform);

        _currentBuildingPrefab = building;
    }

    public void SetCursorAlpha()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = cursorAlpha;
        GetComponent<SpriteRenderer>().color = color;
    }


    public void disableCursor()
    {
        _isActive = false;
        GetComponent<SpriteRenderer>().enabled = false;

    }

    public void enableCursor()
    {
        _isActive = true;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void moveCursorTo(Vector3 WorldPos)
    {
        GetComponent<GridTransform>().MoveToWorldCoords(WorldPos);
    }
}
