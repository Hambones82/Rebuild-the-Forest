using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer 
{
    private float _value = 0;
    private float period;
    private const float defaultPeriod = -1;
    public float Period { set => period = value; }
    private bool enabled = false;
    public bool Enabled { set => enabled = value; }
    public delegate void TimedEvent();
    private event TimedEvent timedEvent;

    public void AddEvent(TimedEvent eventToAdd)
    {
        timedEvent += eventToAdd;
    }

    public void RemoveEvent(TimedEvent eventToRemove)
    {
        timedEvent -= eventToRemove;
    }

    public Timer(float inPeriod = defaultPeriod)
    {
        period = inPeriod;
    }

    public void UpdateTimer()
    {
        if(enabled)
        {
            _value += Time.deltaTime;
            while (_value >= period)
            {
                _value -= period;
                timedEvent?.Invoke();
            }
        }
    }
}
