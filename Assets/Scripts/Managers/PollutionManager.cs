using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


[DefaultExecutionOrder(-3)]
public class PollutionManager : MonoBehaviour, IGameManager
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

    [SerializeField]
    private PollutionTypeController controller;
    public PollutionTypeController PollutionController {  get => controller; }

    private ServiceLocator _serviceLocator;
    public void SelfInit(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null) throw new ArgumentNullException("service locator cannot be null");
        _serviceLocator = serviceLocator;
        _serviceLocator.RegisterService(this);
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            throw new InvalidOperationException("can't have two pollution managers");
        }
    }

    public void MutualInit()
    {
        //set gridMap
        gridMap = FindObjectOfType<GridMap>();  
        controller.Initialize(gridMap, this);
        controller.InitializePollutionState();
        controller.RecalculateFreePositions();//need to write this method -- just use a scan line technique   
    }

    public void UpdateFreePositionsForAddition(Vector2Int cell)
    {        
        controller.UpdateFreePositionsForPollutionAddition(cell);        
    }

    public void UpdateFreePositionsForRemoval(Vector2Int cell)
    {
        controller.UpdateFreePositionsForPollutionRemoval(cell);
    }

    private void Start()
    {
        OnInitComplete?.Invoke();
        //??? <-- for discoverablemanager... might want to just remove that...  
    }    

    private void Update()
    {
        controller.Update();                
    }

    //not sure about having this be public...
    public void AddPollution(Vector2Int cell, PollutionTypeController controller)
    {
        controller.AddPollution(cell);        
        OnPollutionAdded?.Invoke(cell);
    }

    //not sure about having this be public...
    public void RemovePollution(Pollution pollution)
    {
        Vector2Int pollutionPosition = pollution.GetComponent<GridTransform>().topLeftPosMap;
        
        controller.RemovePollution(pollution, pollutionPosition);        
        controller.UpdateFreePositionsForPollutionRemoval(pollutionPosition);        
        OnPollutionDead?.Invoke(pollutionPosition);
    }

    public int GetGraphID(Vector2Int cell)
    {        
        if(controller.GetPollutionAt(cell) != null)
        {
            return controller.GetGraphID(cell);
        }        
        return -1;
    }

    public void RemovePollutionSoft(Pollution pollution)
    {
        Vector2Int pollutionPosition = pollution.GetComponent<GridTransform>().topLeftPosMap;
        controller.RemovePollution(pollution, pollutionPosition);
        controller.UpdateFreePositionsForPollutionRemoval(pollutionPosition);        
    }

    public void NotifyOfSourceDeletion(PollutionSource source)
    {        
        controller.NotifyOfSourceDeletion(source);        
    }
    public int GetPSourceGroupID(PollutionSource psource)
    {                
        return controller.GetPSourceGroupID(psource);            
    }

    public bool IsEffectAtCell(Vector2Int cell, PollutionEffect effect)
    {
        foreach (GridTransform gt in GridMap.Current.GetObjectsAtCell(cell, MapLayer.pollution))
        {
            Pollution subjectPol = gt.GetComponent<Pollution>();
            if (subjectPol != null)
            {
                if (subjectPol.PollutionEffects.Contains(effect))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public string DebugText(Vector2Int cell)
    {
        return controller.DebugText(cell);
    }

}