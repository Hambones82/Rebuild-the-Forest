using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActorUnitManager : MonoBehaviour, IGameManager
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
    public int NumActorUnits { get => actorUnits.Count; }

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


    private ServiceLocator _serviceLocator;
    
    public void SelfInit(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null) throw new ArgumentNullException("service locator cannot be null");
        _serviceLocator = serviceLocator;
        _serviceLocator.RegisterService(this);
        if (_instance != null)
        {
            throw new InvalidOperationException("cannot instantiate more than one BuildingManager");
        }
        _instance = this;
        
        
    }

    //do we really want to reference UI manager with this game element?...  maybe not right?  ui probably is the one that 
    //shoudl know about actor units, not the other way around...
    public void MutualInit()
    {
        
        //
        UIManager.Instance.OnSelectEvent.AddListener(ProcessSelectionEvent);
        UIManager.Instance.OnDeselectEvent.AddListener(ProcessDeselectionEvent);
        //this assumes gridmap is a singleton.  
        gridMap = GridMap.Current;
        actorUnitPool = new GameObjectPool(actorUnitPrefab, parentObj: gridMap.gameObject, activeByDefault: false);
        //what about for test scene...???  
        //register all the actor units on the gridmap with this thing.
        /*
        List<GridTransform> actorUnits = gridMap.GetMapOfType(MapLayer.playerUnits).GetAllObjects();
        foreach(GridTransform gt in actorUnits)
        {
            ActorUnit actor = gt.GetComponent<ActorUnit>();
            if(actor != null)
            {
                RegisterActorUnit(actor);
            }
        }
        */
    }

    //this "invoke" call -- probably can be gotten rid of.  this should work normally based on our initiation code.
    //in other words, our scene gen should just order these things properly - spawn managers, then load UI.
    /*
    private void Start()
    {
        OnInitComplete?.Invoke();
    }
    */
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
        DeRegisterActorUnit(actorUnit);
        OnActorUnitDeath?.Invoke(actorUnit);
        actorUnit.gameObject.SetActive(false);
        actorUnit.ResetWhenKilled();        
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
