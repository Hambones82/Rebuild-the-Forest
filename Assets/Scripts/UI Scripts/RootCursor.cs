using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootCursor : Cursor
{
    private Vector3 origin;
    private float _range;

    public void Initialize(Vector3 worldPos, float range)
    {
        origin = worldPos;
        _range = range;
    }

    public override void moveCursorTo(Vector3 WorldPos)
    {
        Vector3 clampedPos;
        float totalDistance = Vector3.Distance(origin, WorldPos);
        float interpolationWeight = _range / totalDistance;
        if(totalDistance > _range)
        {
            clampedPos = Vector3.Lerp(origin, WorldPos, interpolationWeight);
        }
        else
        {
            clampedPos = WorldPos;
        }
        base.moveCursorTo(clampedPos);
    }
}
