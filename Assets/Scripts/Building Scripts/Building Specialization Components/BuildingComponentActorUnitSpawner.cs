using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentActorUnitSpawner : MonoBehaviour
{
    [SerializeField]
    private float spawnPeriod;

    private Timer spawnTimer;


    private void Awake()
    {
        spawnTimer = new Timer(spawnPeriod);
        spawnTimer.Enabled = true;
        spawnTimer.AddEvent(spawnActorUnit);
    }

    public void spawnActorUnit()
    {
        //need to check if reached max value
        if(!ActorUnitManager.Instance.ActorUnitsFull)
        {
            ActorUnitManager.Instance.SpawnActorUnit(new Vector2Int(10, 10));
        }
    }

    void Update()
    {
        spawnTimer.UpdateTimer();
    }
}
