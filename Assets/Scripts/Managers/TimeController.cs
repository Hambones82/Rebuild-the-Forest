using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour, IGameManager
{
    bool paused = false;

    private ServiceLocator _serviceLocator;
    public void SelfInit(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null) throw new ArgumentNullException("service locator cannot be null");
        _serviceLocator = serviceLocator;
        _serviceLocator.RegisterService(this);
    }

    public void MutualInit() { }

    public void TogglePause()
    {
        //Debug.Log("toggling pause");
        paused = !paused;
        if(paused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
