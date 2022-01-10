using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this guy manages the input.
//what that means is it stores two inputdefinitionmodules: a foreground one and a background one
//it also has the keycodetoinputactiontypes, which correlate actual key presses to input action types
 


//DO I WANT TO ADD A ???
//let's just go w this for now

//problem: need to get all input actions covered by a keycode and try all for the current object.???
public class InputManager : MonoBehaviour {

    //from raw to InputActionType
    //this is some kind of... fixed definitions file.  read in the file, build a map.
    //maybe a... scriptable object
    //from input action type to function call
    public KeyCodeToInputActionTypeDefinitions keyCodeToInputActionTypeDefinitions; //use this to get the action based on input key
    private List<KeyPress> activeKeyPresses;
    //private List<KeyPress> keyPressesActivated;

    private InputDefinitionModule inputDefinitionModule;
    private InputDefinitionModule backgroundInputDefinitionModule;


    private void Awake()
    {
        activeKeyPresses = new List<KeyPress>();
        //keyPressesActivated = new List<KeyPress>();
        //inputDefinitionModule = new InputDefinitionModule(); // default module?  sometimes it's null, sometimes it's something.  probably want a default one.
        //backgroundInputDefinitionModule = new InputDefinitionModule();

        //build a list of keycodes from definitions
        //in other words, what key presses are we looking for?
        foreach (KeyCodeToInputActionTypeDefinitions.KeyCodeToInputActionType kC in keyCodeToInputActionTypeDefinitions.keyCodeToInputActionTypes)
        {
            bool keyPressFound = false;
            foreach(KeyPress keyPressToCheck in activeKeyPresses)
            {
                if(keyPressToCheck.keyCode == kC.keyPress.keyCode)
                {
                    keyPressFound = true;
                }
            }
            if(keyPressFound == false)
            {
                activeKeyPresses.Add(kC.keyPress);
            }
        }
    }

   
	void Update () {
        //add all keys released to keycodespressed
        //keycodes... presses... pressed... get rid of that?
        foreach(KeyPress keyPress in activeKeyPresses)
        {
            if (
               (keyPress.keyPressType == KeyPress.KeyPressType.button_down && Input.GetKeyDown(keyPress.keyCode)) ||
               (keyPress.keyPressType == KeyPress.KeyPressType.button_up && Input.GetKeyUp(keyPress.keyCode))     ||
               (keyPress.keyPressType == KeyPress.KeyPressType.hold && Input.GetKey(keyPress.keyCode))
               )
            {
                InvokeKey(keyPress);   
            }
        }
   
    }

    //different events should add or remove inputdefinitionmodules.  

    //based on the contents of the inputdefinition module and backgroundinputdefinition module, you invoke the associated actions
    public void InvokeKey(KeyPress keyPress)
    {
        //get action based on key
        List<InputActionType> actionsToDo = keyCodeToInputActionTypeDefinitions.GetInputActionTypes(keyPress);
        
        //invoke unityevent based on action
        if(actionsToDo.Count != 0)
        {
            
            if(inputDefinitionModule != null)
            {
                if (inputDefinitionModule.InvokeAction(actionsToDo)) return; //if that and not background passthrough
            }
            if(backgroundInputDefinitionModule != null)
            {
                if (backgroundInputDefinitionModule.InvokeAction(actionsToDo)) return;
            }
            //in this fall back situation, the current input modules do not contain the action that has been requested
        }
    }

    //this should also have an invoke action function so the mouse can take advantage of the action types ???
    //but then how will the mouse transmit its coordinates???
    //so... it'd be nice to have arguments.

    //register the foreground input module
    public void RegisterInputModule(InputDefinitionModule idM)
    {
        inputDefinitionModule = idM;
    }

    //deregister the foreground input module
    public void DeRegisterInputModule(InputDefinitionModule idM) //not really sure about this one... there should always be a context with an input handler, even 
                                                                 //if it doesn't do anything
    {
        if (inputDefinitionModule == idM) inputDefinitionModule = null;
    }

    //register the background input module
    public void RegisterBackgroundInputModule(InputDefinitionModule idM)
    {
        backgroundInputDefinitionModule= idM;
    }

    //deregister the background input module
    public void DeRegisterBackgroundInputModule(InputDefinitionModule idM) //not really sure about this one... there should always be a context with an input handler, even 
                                                                 //if it doesn't do anything
    {
        if (backgroundInputDefinitionModule == idM) backgroundInputDefinitionModule = null;
    }

    //check if the foreground input module is a particular one
    public bool InputModuleIs(InputDefinitionModule idM)
    {
        if (inputDefinitionModule == idM) return true;
        else return false;
    }

    //check if the background input module is a particular one
    public bool BackgroundInputModuleIs(InputDefinitionModule idM)
    {
        if (backgroundInputDefinitionModule == idM) return true;
        else return false;
    }

}
