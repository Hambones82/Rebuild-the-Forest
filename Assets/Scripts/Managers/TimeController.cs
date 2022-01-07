using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    bool paused = false;

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
