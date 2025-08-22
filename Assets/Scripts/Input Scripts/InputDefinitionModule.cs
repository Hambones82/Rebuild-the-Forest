using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//this needs to be a scriptable object

//this allows input definitions to be defined 
//it also allows the funtions defined by the input definitions to be called
//to use this you attach it to any object and define input definitions in the editor
//this is just a definition module -- it correlates inputactiontypes with actions.  
//there is no definition of keypresses (or whatever), those are done by a different layer (e.g., inputmanager and keycodetoinputactiontype)

[System.Serializable]
public class InputDefinitionModule  {

    //has a number of maps from inputactiontype to unityevent
    //it is a pluggable object into input manager
    //input manager will check all of its input definitionmodules to determine what to do.
    [SerializeField] InputDefinitionModuleSO definitions;    
    public bool passThrough = false; //if true and this is the background module, then keys from this one will also cause stuff to happen
    public bool blockUp = false;
    public List<InputDefinition> inputDefinitions;
    private bool _initialized = false;

    private ServiceLocator _serviceLocator;
    public virtual void Initialize(ServiceLocator serviceLocator)
    {
        if(_initialized)
        {
            return;           
        }
        _serviceLocator = serviceLocator;
        if (definitions != null)
        {
            definitions.Initialize(serviceLocator);
            inputDefinitions.Clear();
            foreach(InputDefinition iDefinition in definitions.InputDefinitions)
            {
                inputDefinitions.Add(new InputDefinition(iDefinition.inputActionType, iDefinition.unityEvent));
            }
        }
        
        //copy from so to here
        _initialized = true;
    }
    public bool InvokeAction(List<InputActionType> inputActionType)
    {
        if (!_initialized)
        {
            throw new System.InvalidOperationException("cannot use this input definition module as it has not been initialized");
        }
        bool retVal = false; //whether an action was found here
        if (inputDefinitions == null) return false;
        foreach(InputActionType iAction in inputActionType)
        {
            foreach (InputDefinition iDefinition in inputDefinitions)
            {
                if (iDefinition.inputActionType == iAction)
                {
                    iDefinition.unityEvent.Invoke();
                    retVal = true;  //where true means a function was invoked
                }
            }
        }
        
        //else, no such input action is defined by this
        return retVal;
    }
}
