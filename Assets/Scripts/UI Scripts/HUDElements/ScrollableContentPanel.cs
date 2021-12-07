using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollableContentPanel : MonoBehaviour
{
    [SerializeField]
    protected GameObject contentObjectPrefab;

    protected GameObjectPool contentObjectPool;

    [SerializeField]
    protected RectTransform contentParent; //where the content goes i guess
    protected List<GameObject> contentObjects = new List<GameObject>();

    protected readonly float refreshRate = UITuning.refreshRate;
    protected float refreshCounter = 0;

    [SerializeField] //i guess this is fine?
    protected ActorUnit currentActorUnit;
    public ActorUnit CurrentActorUnit
    {
        set => currentActorUnit = value;
    }

    [SerializeField]
    protected UIManager uIManager;

    protected virtual void Awake()
    {
        contentObjectPool = new GameObjectPool(contentObjectPrefab.gameObject, parentObj: contentParent.gameObject, activeByDefault: false);
        uIManager.OnSelectEvent.AddListener(ProcessSelectionEvent);
        uIManager.OnDeselectEvent.AddListener(ProcessDeselectionEvent);
        //need a hookup for when the current actor unit is set. <-- ?
    }

    protected virtual void ProcessSelectionEvent()
    {
        CurrentActorUnit = uIManager.SelectedGridTransform.GetComponent<ActorUnit>();
        if (currentActorUnit != null)
        {
            SetCachedReferences();
            currentActorUnit.OnDeath.AddListener(ActorUnitDies);
            UpdateContents();
        }
    }

    protected virtual void SetCachedReferences() { }

    protected virtual void ProcessDeselectionEvent()
    {
        ClearButtons();
        if (currentActorUnit != null)
        {
            currentActorUnit.OnDeath.RemoveListener(ActorUnitDies);
            CurrentActorUnit = null;
            //UpdateContents(); //remove all
            foreach (GameObject gobj in contentObjects)
            {
                contentObjectPool.RecycleObject(gobj);
            }
            contentObjects.Clear();//not sure abt this...
            //permittedBuildingTypes.Clear();
            //cachedPermittedBuildingTypes.Clear();
        }
    }

    public virtual void ActorUnitDies()
    {
        ProcessDeselectionEvent(); 
    }

    protected virtual void Update()//maybe we shouldn't do this in reset...?
    {
        refreshCounter += Time.deltaTime;
        if (refreshCounter >= refreshRate)
        {
            UpdateContents();
            refreshCounter = 0;
        }
    }

    protected virtual void UpdateContents() { }

    

    protected virtual void ClearButtons()
    {
        foreach (GameObject gobj in contentObjects)
        {
            contentObjectPool.RecycleObject(gobj);
        }
        contentObjects.Clear();
    }
}
