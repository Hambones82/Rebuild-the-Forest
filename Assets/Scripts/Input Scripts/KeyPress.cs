using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyPress {

    public enum KeyPressType
    {
        button_up,
        button_down,
        hold
    }

    public KeyCode keyCode;
    public KeyPressType keyPressType;
    //modifiers? like shift, ctrl, whatever?

    public bool isTheSameAs(KeyPress target)
    {
        return (target.keyCode == keyCode) && (target.keyPressType == keyPressType);
    }
}
