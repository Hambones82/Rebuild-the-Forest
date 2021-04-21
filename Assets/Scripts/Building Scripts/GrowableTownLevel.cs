using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrowableTownLevel 
{
    public int levelNumber;
    public ResourceSet levelRequirements;
    //???  public ResourceSet buildingOutput; -- modify the Building based on this...?
    public ResourceSet buildingOutput; //i guess how the building should work at the given level...
    public Sprite levelSprite;
    //image to change to?
    //effects??? -- yeah, effects in terms of outputs, etc...
}
