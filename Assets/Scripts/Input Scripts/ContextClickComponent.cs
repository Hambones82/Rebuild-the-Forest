using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ContextClickComponent : MonoBehaviour
{
    public abstract void DoContextClick(Vector2Int mapPosition);
    public virtual void Awake()
    {

    }
}
