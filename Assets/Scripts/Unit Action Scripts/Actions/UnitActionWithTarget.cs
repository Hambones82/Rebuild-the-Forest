using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitActionWithTarget<T> : UnitAction where T : Component
{
    protected T target;

    public virtual void Initialize(GameObject inGameObject, T inComponent)
    {
        base.Initialize(inGameObject);
        target = inComponent;
    }
}
