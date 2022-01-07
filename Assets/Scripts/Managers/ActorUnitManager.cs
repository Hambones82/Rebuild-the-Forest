using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorUnitManager : MonoBehaviour
{
    private static ActorUnitManager _instance;
    public static ActorUnitManager Instance { get => _instance; }

    [SerializeField]
    private int _maxActorUnits = 3;
    public int MaxActorUnits { get => _maxActorUnits; }

    [SerializeField]
    private int _currentActorUnitsCount = 0;
    public int CurrentActorUnitsCount { get => _currentActorUnitsCount; }

    public bool ActorUnitsFull { get { return _currentActorUnitsCount >= _maxActorUnits; } }

    [SerializeField]
    private List<ActorUnit> actorUnits = new List<ActorUnit>();
    public List<ActorUnit> ActorUnits { get => new List<ActorUnit>(actorUnits); }


    [SerializeField]
    private GridMap gridMap;

    [SerializeField]
    private GameObject actorUnitPrefab;

    private GameObjectPool actorUnitPool;
    //make an object pool for actor unit.  i guess it woudl have to be a prefab.

    [SerializeField]
    private ActorUnit selectedActorUnit;

    public delegate void ActorUnitLifecycleDelegate(ActorUnit actorUnit);
    public event ActorUnitLifecycleDelegate OnActorUnitDeath;
    public event ActorUnitLifecycleDelegate OnActorUnitSpawn;
    public event Action OnInitComplete;

    private void Awake()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("cannot instantiate more than one BuildingManager");
        }
        _instance = this;
        actorUnitPool = new GameObjectPool(actorUnitPrefab, parentObj: gridMap.gameObject, activeByDefault: false);
        //register all the actor units on the gridmap with this thing.
        List<GridTransform> actorUnits = gridMap.GetMapOfType(MapLayer.playerUnits).GetAllObjects();
        foreach(GridTransform gt in actorUnits)
        {
            ActorUnit actor = gt.GetComponent<ActorUnit>();
            if(actor != null)
            {
                RegisterActorUnit(actor);
            }
        }
        UIManager.Instance.OnSelectEvent.AddListener(ProcessSelectionEvent);
        UIManager.Instance.OnDeselectEvent.AddListener(ProcessDeselectionEvent);
    }

    private void Start()
    {
        OnInitComplete?.Invoke();
    }

    public void CancelActorUnitActions()
    {
        selectedActorUnit.GetComponent<UnitActionController>().CancelAllActions();
    }

    private void ProcessSelectionEvent()
    {
        selectedActorUnit = UIManager.Instance.SelectedGridTransform.GetComponent<ActorUnit>();
    }

    private void ProcessDeselectionEvent()
    {
        selectedActorUnit = null;
    }

    private void RegisterActorUnit(ActorUnit unit)
    {
        actorUnits.Add(unit);
        _currentActorUnitsCount++;
    }

    private void DeRegisterActorUnit(ActorUnit unit)
    {
        actorUnits.Remove(unit);
        _currentActorUnitsCount--;
    }
    

    public void KillActorUnit(ActorUnit actorUnit)
    {
        OnActorUnitDeath?.Invoke(actorUnit);
        actorUnit.gameObject.SetActive(false);
        actorUnit.ResetWhenKilled();
        DeRegisterActorUnit(actorUnit);
        actorUnitPool.RecycleObject(actorUnit.gameObject);
    }

    public void SpawnActorUnit(Vector2Int coords)
    {
        ActorUnit newActorUnit = actorUnitPool.GetGameObject().GetComponent<ActorUnit>();
        newActorUnit.gameObject.SetActive(true);
        newActorUnit.GetComponent<GridTransform>().MoveToMapCoords(coords);
        RegisterActorUnit(newActorUnit);
        OnActorUnitSpawn?.Invoke(newActorUnit);
    }
}
