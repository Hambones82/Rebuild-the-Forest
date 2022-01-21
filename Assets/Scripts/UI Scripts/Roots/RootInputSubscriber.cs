using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootInputSubscriber : InputSubscriber
{
    private RootBuildingComponent rootComponent;

    private void Awake()
    {
        rootComponent = GetComponent<RootBuildingComponent>();
    }

    public void StartRootTool()
    {
        UIManager.Instance.SwitchMouseTool(GrowRootTool.Instance, rootComponent);
    }
}
