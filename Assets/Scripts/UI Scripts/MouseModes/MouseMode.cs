using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MouseMode
{
    public abstract MouseMode LeftClick(Vector3 clickPoint, UIManager uiManager);
    public abstract MouseMode RightClick(Vector3 clickPoint, UIManager uiManager);
}
