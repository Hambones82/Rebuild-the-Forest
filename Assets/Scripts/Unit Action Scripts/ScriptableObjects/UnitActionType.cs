using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitActionType : ObjectType<UnitAction>
{
    [SerializeField]
    protected bool _performInAdjacentSquare;
    public bool PerformInAdjacentSquare { get => _performInAdjacentSquare; }
}
