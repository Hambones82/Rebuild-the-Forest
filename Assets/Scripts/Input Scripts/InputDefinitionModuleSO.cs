using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputDefinitionModuleSO : ScriptableObject
{
    public bool passThrough = false; //if true and this is the background module, then keys from this one will also cause stuff to happen
    public bool blockUp = false;
    protected List<InputDefinition> inputDefinitions;
    private bool _initialized = false;

    private ServiceLocator _serviceLocator;

    public IEnumerable<InputDefinition> InputDefinitions
    {
        get
        {
            foreach (var definition in inputDefinitions)
            {
                yield return definition;
            }
        }
    }

    public void RegisterInputDefinition(InputDefinition definition)
    {
        inputDefinitions.Add(definition);
    }

    public virtual void Initialize(ServiceLocator serviceLocator)
    {
        if (_initialized)
        {
            return;
        }
        _serviceLocator = serviceLocator;
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
        foreach (InputActionType iAction in inputActionType)
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
