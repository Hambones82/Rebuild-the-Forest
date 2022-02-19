using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//attach to an object with rootbuildingcomponent
//need to attach as child the thing that will actually be visualized.  set that thing's position, enable/disable.
public class RootGrowthTargetVisualizer : MonoBehaviour
{
    [SerializeField]
    private GameObject visualizerObject; //just has a sprite renderer and a grid transform
    private GridTransform thisGridTransform;
    private GridTransform visualizerObjectGT;
    RootBuildingComponent rbc;
    private MouseSelector mouseSelector;

    private void Awake()
    {
        //hook up to the rootbuildingcomponent's events for...  changing root growth target.
        //that's when we'd move the visualizer object
        //also... selection...  enable the object on select.
        rbc = GetComponent<RootBuildingComponent>();
        rbc.OnTargetChangeEvent += ChangeTarget;
        rbc.OnGrowthEnd += UnShowVisualizer;
        rbc.OnGrowthStart += ShowVisualizer;
        visualizerObject = GameObject.FindGameObjectWithTag("RootTargetVisualizer");
        visualizerObjectGT = visualizerObject.GetComponent<GridTransform>();
        if(visualizerObjectGT == null)
        {
            throw new InvalidOperationException("visualizer object grid transform is null");
        }
        mouseSelector = GetComponent<MouseSelector>();
        mouseSelector.OnSelect.AddListener(ShowVisualizer);
        mouseSelector.OnDeselect.AddListener(UnShowVisualizer);
        thisGridTransform = GetComponent<GridTransform>();
        visualizerObjectGT.MoveToMapCoords(thisGridTransform.topLeftPosMap);
        UnShowVisualizer();
    }

    private void ChangeTarget(Vector2Int newTarget)
    {
        visualizerObjectGT.MoveToMapCoords(newTarget);
    }

    private void ShowVisualizer()
    {
        if(rbc.Growing && mouseSelector.IsSelected)
        {
            visualizerObject.SetActive(true);
        }
    }

    private void UnShowVisualizer()
    {
        visualizerObject.SetActive(false);
    }
}
