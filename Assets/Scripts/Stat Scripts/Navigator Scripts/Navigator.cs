using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//regarding the actor unit and the mouse selector...  maybe we want to give this an input module and allow that input module to set a certain action
//when a certain mouse button is pressed or something...  i'm not sure...
[RequireComponent(typeof(GridTransform))]
public class Navigator : MonoBehaviour
{
    //need a speed
    [SerializeField] private float speed;
    public float Speed { get => speed; }
    [SerializeField]
    private NavigatorStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = new NavigatorStateMachine(gameObject);
    }

    public void MoveTo(Vector2Int targetMapCoords)
    {
        stateMachine.SetTarget(targetMapCoords);
    }

    private void Update()
    {
        stateMachine.AdvanceTime(Time.deltaTime);//probably / maybe change the delta time later, but for now this is fine.
    }
    //how about a state machine... two states --> moving, standing still...
}
