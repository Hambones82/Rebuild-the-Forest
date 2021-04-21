using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this allows you to edit, offline, the correlation between keycodes and inputactiontypes
[CreateAssetMenu(fileName = "KeyCodeToActionDefinitions", menuName = "Keycode Definitions")]
public class KeyCodeToInputActionTypeDefinitions : ScriptableObject {

    [System.Serializable]
	public class KeyCodeToInputActionType
    {
        //public KeyCode keyCode;
        public KeyPress keyPress;
        public InputActionType inputActionType;
    }

    public List<KeyCodeToInputActionType> keyCodeToInputActionTypes;

    //this function returns the input actions for a given keypress
    public List<InputActionType> GetInputActionTypes(KeyPress keyPress)
    {
        List<InputActionType> inputActionTypes = new List<InputActionType>();
        foreach(KeyCodeToInputActionType kcType in keyCodeToInputActionTypes)
        {
            if(kcType.keyPress.isTheSameAs(keyPress))
            {
                inputActionTypes.Add(kcType.inputActionType);
            }
        }
        return inputActionTypes;
    }

}
