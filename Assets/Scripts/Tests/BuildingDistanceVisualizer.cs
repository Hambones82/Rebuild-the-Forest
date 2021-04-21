using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//just get rid of this...
//get rid of the tile map visualizer enum and associated things...
//just debug.log the calculated distance when we make a connection.
public class BuildingDistanceVisualizer : MonoBehaviour
{
    //use a prefab with a sprite and a grid transform.  use that to place the sprite at a given map location
    // Start is called before the first frame update
    [SerializeField]
    private UIManager uIManager;
    [SerializeField]
    private TileDataMap tdMap;

    void Awake()
    {
        uIManager.OnSelectEvent.AddListener(ShowBuildingRange);
        uIManager.OnDeselectEvent.AddListener(RemoveBuildingRange);
    }

    public void ShowBuildingRange()
    {
        //Debug.Log("Showing");
        GridTransform subject = uIManager.SelectedGridTransform;
        Building building = subject.GetComponent<Building>();
        if (building == null)
        {
            return;
        }
        int range = building.maxConnectionLength;
        //need to set the map visualizer based on this information...
        //maybe make a "mock up" building with size selectable in the inspector?
    }

    public void RemoveBuildingRange()
    {
        //Debug.Log("Unshowing");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
