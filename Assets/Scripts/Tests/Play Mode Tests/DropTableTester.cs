using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropTableTester : MonoBehaviour
{
    [SerializeField]
    private DropTable dropTable;

    private float timerVal = 0;
    private float timerPeriod = .01f;

    private void Awake()
    {
        dropTable.Initialize();
    }

    private void Update()
    {
        timerVal += Time.deltaTime;
        if(timerVal >= timerPeriod)
        {
            //Debug.Log($"Got random item: {dropTable.GetDroppedBuildingType()?.name}");
            dropTable.GetDroppedBuildingType();
            timerVal = 0;
        }
    }
}
