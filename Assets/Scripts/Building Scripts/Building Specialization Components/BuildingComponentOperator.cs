using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingComponentOperator : MonoBehaviour
{
    [SerializeField]
    protected float _operateTimer = 0;

    [SerializeField]
    protected float _operateTimerTrigger;

    [SerializeField]
    protected bool _continuous;

    public virtual bool Operate(float dt, GameObject inGameObject = null)
    {
        //Debug.Log("operator component: operate");
        if (!IsOperatable(inGameObject)) return false;
        _operateTimer += dt;
        bool triggered = false;
        while (_operateTimer >= _operateTimerTrigger)
        {
            Trigger(inGameObject);
            triggered = true;
            _operateTimer -= _operateTimerTrigger;
            if(!_continuous)
            {
                break;
            }
        }
        return !triggered || _continuous;
    }

    protected abstract void Trigger(GameObject gameObject = null);
    
    public abstract bool IsOperatable(GameObject inGameobject);
}
