using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//here is the paradigm for using these:
//these input subscribers should be used only to call into argument-less functions of the object this is attached to.
//the sole purpose of these actions is to decouple the actual assignment of keyboard codes to the functions
public class InputSubscriber : MonoBehaviour {

    public enum InputDefinitionModuleType { background, foreground }
    public InputDefinitionModuleType inputDefinitionModuleType = InputDefinitionModuleType.foreground;
    public InputManager inputManager;
    public InputDefinitionModule inputDefinitionModule;

    //when this is activated, send the inputdefinitionmodule to the input manager for use.  

    public void Activate()//you'd activate this when you select the object.
    {
        if(inputDefinitionModuleType == InputDefinitionModuleType.foreground)
        {
            inputManager.RegisterInputModule(inputDefinitionModule);
        }
        else
        {
            inputManager.RegisterBackgroundInputModule(inputDefinitionModule);
        }
    }

    public void DeActivate()
    {
        if (inputDefinitionModuleType == InputDefinitionModuleType.foreground)
        {
            inputManager.DeRegisterInputModule(inputDefinitionModule);
        }
        else
        {
            inputManager.DeRegisterBackgroundInputModule(inputDefinitionModule);
        }
    }

	// Use this for initialization
	void Start () {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        if(inputDefinitionModuleType == InputDefinitionModuleType.background) Activate();
        //inputDefinitionModule.inputDefinitions = new List<InputDefinition>(); //not correct... something else...
    }
	

    private void Awake()
    {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
    }
}
