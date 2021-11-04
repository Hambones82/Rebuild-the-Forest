using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MouseTool
{
    public virtual void StartTool(UIManager uiManager, UnityEngine.Object unityObject1 = null, UnityEngine.Object unityObject2 = null) { }
    public virtual void EndTool(UIManager uiManager) { }
    public abstract bool LeftClick(Vector3 mousePosition, UIManager uiManager);
    public abstract bool RightClick(Vector3 mousePosition, UIManager uiManager);
    public abstract void Cancel();
}
