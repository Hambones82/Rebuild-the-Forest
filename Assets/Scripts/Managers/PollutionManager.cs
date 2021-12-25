using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//this is done in a very naive way, could be optimized... not necessary for prototype
[DefaultExecutionOrder(-3)]
public class PollutionManager : MonoBehaviour
{
    public delegate void PollutionEvent(Vector2Int cell);
    public event Action OnInitComplete;
    public event PollutionEvent OnPollutionDead;

    private static PollutionManager _instance;
    public static PollutionManager Instance
    {
        get => _instance;
    }

    [SerializeField]
    private GridMap gridMap;

    [SerializeReference, SubclassSelector]
    private List<PollutionTypeController> pollutionControllers;
    
    private void Awake()
    {
        foreach(PollutionTypeController controller in pollutionControllers)
        {
            controller.Initialize(gridMap, this);
        }

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            throw new InvalidOperationException("can't have two pollution managers");
        }
    }

    private void Start()
    {
        OnInitComplete?.Invoke();
    }

    private void Update()
    {
        foreach (PollutionTypeController controller in pollutionControllers)
        {
            controller.UpdateState();
        }
    }

    public void RemovePollution(Pollution pollution)
    {
        Vector2Int pollutionPosition = pollution.GetComponent<GridTransform>().topLeftPosMap;
        //for now, do this for all... but should really find the correct one...
        foreach (PollutionTypeController controller in pollutionControllers)
        {
            controller.RemovePollution(pollution, pollutionPosition);
        }
        OnPollutionDead(pollutionPosition);
    }

}