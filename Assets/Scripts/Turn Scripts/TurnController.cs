using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnController : MonoBehaviour
{

    //turn controller -- singleton???  maybe a normal class singleton with turn controller something.  actually this doesn't need to be a gameobject
    //so may...  anyway... not sure how to do this.  need to figure it out...
    private UnityAction endTurnCallbacks;

    public void AddEndTurnCallback(UnityAction endTurnCallback)
    {
        endTurnCallbacks += endTurnCallback;
    }

    public void EndTurn()
    {
        if(endTurnCallbacks == null)
        {
            Debug.Log("end turn callbacks is null?");
        }
        endTurnCallbacks.Invoke();
    }
}
