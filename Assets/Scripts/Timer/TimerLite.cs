using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerLite 
{
    private float timerValue = 0;
    private float period;

    public TimerLite(float inPeriod)
    {
        period = inPeriod;
    }

    public void Reset()
    {
        timerValue = 0;
    }
    public bool UpdateTimer(float dt)
    {
        timerValue += dt;
        if(timerValue >= period)
        {
            timerValue -= period;
            return true;
        }
        else
        {
            return false;
        }
    }
}
