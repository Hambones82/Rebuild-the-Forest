using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorUnitPollutionStats : MonoBehaviour
{
    [SerializeField]
    private float cleaningSpeed;
    public float CleaningSpeed { get => cleaningSpeed; }
    [SerializeField]
    private float perCleanAmount;
    public float PerCleanAmount { get => perCleanAmount; }
    public void ImproveSpeed(float dt)
    {
        cleaningSpeed += dt;
    }
}
