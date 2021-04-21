using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//IMapDisplayable: a grid sub map type of thing that lets you do what's below.
//usefully, it has a map change event
public interface IMapDisplayable
{
    bool IsCellOccupied(Vector2Int cellToCheck);
    Vector2Int GetSize();
    event Action mapChangeEvent;
}

