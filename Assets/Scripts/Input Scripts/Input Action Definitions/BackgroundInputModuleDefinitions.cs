using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.SceneView;

[CreateAssetMenu(fileName = "BackgroundInputs", menuName = "ScriptableObjects/Inputs/BackgroundInputs")]
public class BackgroundInputModuleDefinitions : InputDefinitionModuleSO
{
    private void RegisterUnityEvent(InputActionType actionType, Action action)
    {
        
        UnityEvent uEvent = new UnityEvent();
        uEvent.AddListener(new UnityAction(action));
        inputDefinitions.Add(new InputDefinition
            (
            actionType,
            uEvent
            ));
    }

    public override void Initialize(ServiceLocator serviceLocator)
    {
        base.Initialize(serviceLocator);
        //this is just a code replixa of the settings from the in-scene prefab
        inputDefinitions.Clear();
        CameraMover cameraMover = Camera.main.GetComponent<CameraMover>();
        if(cameraMover == null)
        {
            throw new System.InvalidOperationException("cannot begin without a camera");
        }

        RegisterUnityEvent(InputActionType.camera_scroll_up,cameraMover.moveCameraUp);
        RegisterUnityEvent(InputActionType.camera_scroll_down, cameraMover.moveCameraDown);
        RegisterUnityEvent(InputActionType.camera_scroll_left, cameraMover.moveCameraLeft);
        RegisterUnityEvent(InputActionType.camera_scroll_right, cameraMover.moveCameraRight);

        ActorUnitManager actorUnitManager = serviceLocator.LocateService<ActorUnitManager>();
        RegisterUnityEvent(InputActionType.cancel_action, actorUnitManager.CancelActorUnitActions);

        KeyboardUnitSelector keyboardUnitSelector = serviceLocator.LocateService<KeyboardUnitSelector>();
        RegisterUnityEvent(InputActionType.select_unit_1, keyboardUnitSelector.SelectUnit1);
        RegisterUnityEvent(InputActionType.select_unit_2, keyboardUnitSelector.SelectUnit2);
        RegisterUnityEvent(InputActionType.select_unit_3, keyboardUnitSelector.SelectUnit3);

        TimeController timeController = serviceLocator.LocateService<TimeController>();
        RegisterUnityEvent(InputActionType.pause_toggle, timeController.TogglePause);
    }
}
