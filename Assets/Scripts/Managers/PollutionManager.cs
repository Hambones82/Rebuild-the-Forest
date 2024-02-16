using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//pollution manager needs a "change" or "overwrite" that doesn't cause the drop to be triggered...
//do all the initialization for the controllers.
//have flat controllers...
[DefaultExecutionOrder(-3)]
public class PollutionManager : MonoBehaviour
{
    [SerializeField]
    private SlowEffect _slowEffect;
    public SlowEffect SlowEffect { get => _slowEffect; }

    [SerializeField]
    private StopEffect _stopEffect;
    public StopEffect StopEffect { get => _stopEffect; }

    [SerializeField]
    private DamageEffect _damageEffect;
    public DamageEffect DamageEffect { get => _damageEffect; }

    public delegate void PollutionEvent(Vector2Int cell);
    public event Action OnInitComplete;
    public event PollutionEvent OnPollutionDead;
    public event PollutionEvent OnPollutionAdded;

    private static PollutionManager _instance;
    public static PollutionManager Instance
    {
        get => _instance;
    }

    [SerializeField]
    private GridMap gridMap;

    [SerializeReference, SubclassSelector]
    private List<PollutionTypeController> pollutionControllers;
    public List<PollutionTypeController> PollutionControllers { get => pollutionControllers; }
    
    //so maybe we do keep the type controllers... would make it easier i think...
    private void Awake()//so initialize...  go from highest to lowest priority...
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            throw new InvalidOperationException("can't have two pollution managers");
        }
        foreach (PollutionTypeController controller in pollutionControllers)
        {
            controller.OnPollutionAdd += UpdateFreePositionsForAddition;
            controller.OnPollutionAdd += AddPollution;
            controller.OnPollutionDelete += UpdateFreePositionsForRemoval;
            controller.Initialize(gridMap, this);
        }
        foreach(PollutionTypeController controller in pollutionControllers)
        {
            controller.InitializePollutionState();
        }
    }

    private void AddPollution(Vector2Int cell, int priority)
    {
        OnPollutionAdded?.Invoke(cell);
    }

    public void UpdateFreePositionsForAddition(Vector2Int cell, int priority)
    {
        foreach (PollutionTypeController controller in pollutionControllers)
        {
            controller.UpdateFreePositionsForAddition(cell, priority);
        }
    }

    public void UpdateFreePositionsForRemoval(Vector2Int cell, int priority)
    {
        foreach (PollutionTypeController controller in pollutionControllers)
        {
            controller.UpdateFreePositionsForRemoval(cell, priority);
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
        OnPollutionDead?.Invoke(pollutionPosition);
    }

    public void RemovePollutionSoft(Pollution pollution)
    {
        Vector2Int pollutionPosition = pollution.GetComponent<GridTransform>().topLeftPosMap;
        //for now, do this for all... but should really find the correct one...
        foreach (PollutionTypeController controller in pollutionControllers)
        {
            controller.RemovePollution(pollution, pollutionPosition);
        }
    }

    public bool IsEffectAtCell(Vector2Int cell, PollutionEffect effect)
    {
        foreach (GridTransform gt in GridMap.Current.GetObjectsAtCell(cell, MapLayer.pollution))
        {
            Pollution subjectPol = gt.GetComponent<Pollution>();
            if (subjectPol != null)
            {
                if (subjectPol.pollutionEffects.Contains(effect))
                {
                    return true;
                }
            }
        }
        return false;
    }

}