using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


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

    [SerializeField]
    private PollutionTypeController controller;
    public PollutionTypeController PollutionController {  get => controller; }

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
        controller.Initialize(gridMap, this);
        controller.InitializePollutionState();        
        controller.RecalculateFreePositions();//need to write this method -- just use a scan line technique        
    }


    public void UpdateFreePositionsForAddition(Vector2Int cell, int priority)
    {        
        controller.UpdateFreePositionsForPollutionAddition(cell, priority);        
    }

    public void UpdateFreePositionsForRemoval(Vector2Int cell, int priority)
    {
        controller.UpdateFreePositionsForPollutionRemoval(cell, priority);
    }

    private void Start()
    {
        OnInitComplete?.Invoke();
        //??? <-- for discoverablemanager... might want to just remove that...  
    }    

    private void Update()
    {    
        if (!controller.CheckForTick())
        {
            return;
        }
            
        List<Vector2Int> candidateCellsToAdd = controller.GetCandidateCellsToAddPollution();
        List<Vector2Int> confirmedCellsToAdd = new List<Vector2Int>();
        List<Pollution> pollutionsToRemove = new List<Pollution>();
        foreach (Vector2Int candidateCell in candidateCellsToAdd)
        {                
            Pollution pollutionAtTargetCell = GridMap.Current.GetObjectAtCell<Pollution>(candidateCell, MapLayer.pollution);
            bool blockedByPriority = controller.BlockedByPriorityOf(pollutionAtTargetCell);
            bool blockedByEffect = controller.BlockedByEffect(candidateCell);                
            if (!blockedByPriority && !blockedByEffect) 
            {
                if(pollutionAtTargetCell) pollutionsToRemove.Add(pollutionAtTargetCell);
                confirmedCellsToAdd.Add(candidateCell);
            }
            if (blockedByEffect && !blockedByPriority)
            {                
                controller.PollutionPrefab.OnSpawnBlocked(candidateCell);
            }
        }
        foreach (Vector2Int cell in confirmedCellsToAdd)
        {
            AddPollution(cell, controller);
            UpdateFreePositionsForAddition(cell, controller.PollutionPrefab.Priority);                
        }
        foreach (Pollution pollution in pollutionsToRemove)
        {
            RemovePollutionSoft(pollution);
            UpdateFreePositionsForRemoval(pollution.GetComponent<GridTransform>().topLeftPosMap, pollution.Priority);                
        }                
    }

    private void AddPollution(Vector2Int cell, PollutionTypeController controller)
    {
        controller.AddPollution(cell);        
        OnPollutionAdded?.Invoke(cell);
    }
    

    public void RemovePollution(Pollution pollution)
    {
        Vector2Int pollutionPosition = pollution.GetComponent<GridTransform>().topLeftPosMap;
        
        controller.RemovePollution(pollution, pollutionPosition);        
        controller.UpdateFreePositionsForPollutionRemoval(pollutionPosition, pollution.Priority);        
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
        controller.UpdateFreePositionsForPollutionRemoval(pollutionPosition, pollution.Priority);        
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

}