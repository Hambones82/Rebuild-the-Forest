using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Cursor : MonoBehaviour
{
    protected bool _isActive = false;
    public bool isActive
    {
        get { return _isActive; }
    }

    public float cursorAlpha = .5f;

    protected virtual void Update()
    {
        if (_isActive)
        {
            moveCursorTo(MouseManager.Instance.MouseWorldPosition);
        }
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
