using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActorUnitSpeed : MonoBehaviour
{
    [SerializeField]
    private float speed;
    public float Speed { get => speed; }
    public void ImproveSpeed(float dt)
    {
        speed += dt;
    }
}
