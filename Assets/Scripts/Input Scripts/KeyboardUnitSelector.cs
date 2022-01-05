using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class KeyboardUnitSelector : MonoBehaviour
{
    [SerializeField]
    private int numSelectables;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    ActorUnitManager actorUnitManager;
    [SerializeField]
    private ActorUnit[] keyboardSelectableActorUnits;

    private void Start()
    {
        keyboardSelectableActorUnits = new ActorUnit[numSelectables];
        int count = 0;
        foreach(ActorUnit actor in actorUnitManager.ActorUnits)
        {
            keyboardSelectableActorUnits[count++] = actor;
        }
        actorUnitManager.OnActorUnitDeath += ActorUnitDies;
        actorUnitManager.OnActorUnitSpawn += ActorUnitSpawned;
    }

    private void ActorUnitDies(ActorUnit actor)
    {
        int index = Array.IndexOf(keyboardSelectableActorUnits, actor);
        if (index != - 1)
        {
            keyboardSelectableActorUnits[index] = null;
        }
    }

    private void ActorUnitSpawned(ActorUnit actor)
    {
        int index = Array.IndexOf(keyboardSelectableActorUnits, null);
        if(index != -1)
        {
            keyboardSelectableActorUnits[index] = actor;
        }
    }

    private void SelectUnit(int unitNum)
    {
        if (unitNum >= numSelectables) return;
        GridTransform gtToSelect = keyboardSelectableActorUnits[unitNum]?.GetComponent<GridTransform>();
        if(gtToSelect != null)
        {
            uiManager.HardSelectGridTransform(gtToSelect);
        }
    }

    public void SelectUnit1() => SelectUnit(0);
    
    public void SelectUnit2() => SelectUnit(1);

    public void SelectUnit3() => SelectUnit(2);

}
