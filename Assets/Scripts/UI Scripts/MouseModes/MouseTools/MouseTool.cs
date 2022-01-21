using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MouseTool
{
    public abstract void StartTool(UnityEngine.Object unityObject1 = null, UnityEngine.Object unityObject2 = null);
    public abstract void EndTool();
    public abstract bool LeftClick(Vector3 mousePosition);
    public abstract bool RightClick(Vector3 mousePosition);
}
