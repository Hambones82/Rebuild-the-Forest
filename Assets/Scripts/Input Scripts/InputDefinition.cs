using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//this is just a class that associates inputactiontypes with unityevents.  i think the purpose is that when a particular input action type is invoked, the proper unity event is invoked.
//in other words, this associates our defined enums with actual actions.
//the InputDefinitionModule class, used in the inputManager GameObject class, includes a list of these InputDefinitions
[System.Serializable]
public class InputDefinition {
    public InputActionType inputActionType;
    public UnityEvent unityEvent;

    public InputDefinition(InputActionType inputActionType, UnityEvent unityEvent)
    {
        this.inputActionType = inputActionType;
        this.unityEvent = unityEvent;
    }

    //string tooltip
    //string action name
    //then you can auto-generate buttons based on this information.  
    //button is labeled with hotkey and action name
    //on mouse over causes tooltip to appear
    //need to of course load the keys into the keyboard manager upon hitting select... but not too hard, i already have a function for that.  deselect would do the opposite.
}
