using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionVisualizer : MonoBehaviour
{
    public ConnectionIndicator indicatorPrefab;
    private List<ConnectionIndicator> indicators = new List<ConnectionIndicator>();
    public Building building;

    public void Awake()
    {
        building = GetComponent<Building>();
        building.AddCallbacks(ChangeIndicator, AddIndicator, RemoveIndicator);
        //building.setcallbacks...
    }

    public void AddIndicator(BuildingConnection c)
    {
        if (c.connectionDirection == BuildingConnection.ConnectionDirection.output) return;
        Vector3 thisPosition = building.transform.position;
        
        Vector3 thatPosition = c.ConnectedBuilding.transform.position;
        
        //calculate mid position
        Vector3 midPoint = (thisPosition + thatPosition) / 2;
        
        ConnectionIndicator newIndicator = Instantiate(indicatorPrefab);
        indicators.Add(newIndicator);
        newIndicator.transform.position = midPoint;

        var diff = thatPosition - thisPosition;
        var ratio = diff.y / diff.x;
        var angleRad = Mathf.Atan(ratio);
        //angle isn't exactly right... bc it's not pointing correctly... always points down.
        var setAngle = Mathf.Rad2Deg * angleRad;
        if (thatPosition.x >= thisPosition.x) setAngle += 180;
        var rot = newIndicator.transform.eulerAngles;
        var newRot = new Vector3(rot.x, rot.y, setAngle);
        newIndicator.transform.eulerAngles = newRot;
        //last thing to do is just set the size correctly...
        var newSize = newIndicator.GetComponent<SpriteRenderer>().size;
        float distance = Vector3.Distance(thatPosition, thisPosition);
        newSize.x = distance;
        newIndicator.GetComponent<SpriteRenderer>().size = newSize;
    }

    public void ChangeIndicator(BuildingConnection c)
    {

    }

    public void RemoveIndicator(BuildingConnection c)
    {

    }

    //setup arrow at position
    //remove arrow at position...
    //just do... on awake, add a function of this to the building's OnConnectionChange event
}
