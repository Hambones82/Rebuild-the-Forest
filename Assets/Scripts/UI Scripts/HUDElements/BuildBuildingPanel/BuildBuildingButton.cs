using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BuildBuildingButton : MonoBehaviour
{
    //the button will start the place building tool.
    //so we probably need to store some state -- what building type this button is for
    //we also need a function -- start the tool with the building type.

    private TMPro.TextMeshProUGUI _text;
    public TMPro.TextMeshProUGUI Text { get => _text; }

    private void Awake()
    {
        _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (_text == null)
            Debug.Log("text is null");
    }

    private ActorUnit actorUnit;
    public ActorUnit ActorUnit
    {
        get => actorUnit;
        set => actorUnit = value;
    }

    private BuildingType bType;
    public BuildingType BType
    {
        set => bType = value;
    }

    public void ButtonClick()
    {
        UIManager.Instance.SwitchMouseTool(PlaceBuildingTool.Instance, bType, actorUnit);
    }
}
