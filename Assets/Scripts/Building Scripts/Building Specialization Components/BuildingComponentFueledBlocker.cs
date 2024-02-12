using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingComponentFueledBlocker : MonoBehaviour
{
    [SerializeField]
    private Inventory inventory;
    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        Assert.IsNotNull(inventory, "Fueled Blocker component requires inventory.");
        // inventory should have...  callbacks for going from 0 to something and from something to 0
        // subscribe to those callbacks, call the unity action for each one.  unity action willl need to have an argument specified
        // specify enable/disable for this...

    }


}
