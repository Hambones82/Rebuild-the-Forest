using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingComponentOperator : MonoBehaviour
{
    public abstract bool Operate(float dt);
    public abstract bool IsOperatable();
}
