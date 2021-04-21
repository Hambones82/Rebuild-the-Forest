using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandPanelTextElement : MonoBehaviour {

    public Text labelText;
    public Text valueText;
    

    public void SetLabel(string label)
    {
        labelText.text = label;
    }

    public void SetValue(string value)
    {
        valueText.text = value;
    }
}
