using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//no need?
public class PlayerController : MonoBehaviour
{
    public PlayerResourceData playerData;
    /*
    [SerializeField]
    private Money money;
    public Money Money
    {
        get => money;
    }
    */

    public bool CanPlaceBuilding(Building building)
    {
        return playerData.CanBuild(building);
    }
    //maybe a buyer here? -- ok... player would  need money, we could ask the buyer about stuff...  buyer should be 
}
