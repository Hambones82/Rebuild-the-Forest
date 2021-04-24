using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class UnitAction 
{
    protected string actionName = "NO_ACTION_ENTERED";
    public string ActionName { get => actionName; }
    protected GameObject gameObject;
    public GameObject GameObject { get => gameObject; set => gameObject = value; }

    public virtual void Initialize(GameObject inGameObject)
    {
        gameObject = inGameObject;
    }

    public abstract void StartAction();

    public abstract void EndAction();

    public abstract void ImproveStat(float ImproveAmount);

    //whether to continue the current action
    public abstract bool AdvanceAction(float dt); //advance action, have it do it's thing for the current frame...  
}
