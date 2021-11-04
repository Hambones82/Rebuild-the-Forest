using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//remove actor unit reference after building.  think about exactly how this works...  need to figure out what layer needs to do this
//also need to ensure the correct order.  make sure the buttons don't dance.

public class BuildBuildingPanel : MonoBehaviour
{
    [SerializeField]
    private BuildBuildingButton buildBuildingButtonTemplatePrefab;

    private GameObjectPool buildBuildingButtonPool;

    [SerializeField]
    private RectTransform contentParent;
    private List<BuildBuildingButton> childButtons = new List<BuildBuildingButton>();

    private readonly float refreshRate = UITuning.refreshRate;
    private float refreshCounter = 0;

    [SerializeField]
    private ActorUnit currentActorUnit;
    public ActorUnit CurrentActorUnit
    {
        set => currentActorUnit = value;
    }

    [SerializeField]
    private UIManager uIManager;

    private void Awake()
    {
        buildBuildingButtonPool = new GameObjectPool(buildBuildingButtonTemplatePrefab.gameObject, parentObj: contentParent.gameObject, activeByDefault: false);
        uIManager.OnSelectEvent.AddListener(ProcessSelectionEvent);
        uIManager.OnDeselectEvent.AddListener(ProcessDeselectionEvent);
        //need a hookup for when the current actor unit is set. <-- ?
    }

    private void ProcessSelectionEvent()
    {
        CurrentActorUnit = uIManager.SelectedGridTransform.GetComponent<ActorUnit>();
        if (currentActorUnit != null)
        {
            currentActorUnit.OnDeath.AddListener(ActorUnitDies);
            UpdateContents();
        }
    }

    private void ProcessDeselectionEvent()
    {
        if(currentActorUnit != null)
        {
            currentActorUnit.OnDeath.RemoveListener(ActorUnitDies);
            CurrentActorUnit = null;
            //UpdateContents(); //remove all
            foreach (BuildBuildingButton button in childButtons)
            {
                buildBuildingButtonPool.RecycleObject(button.gameObject);
            }
            childButtons.Clear();//not sure abt this...
            permittedBuildingTypes.Clear();
            cachedPermittedBuildingTypes.Clear();
        }
    }

    public void ActorUnitDies()
    {
        ProcessDeselectionEvent();
    }

    private void Update()//maybe we shouldn't do this in reset...?
    {
        refreshCounter += Time.deltaTime;
        if(refreshCounter>=refreshRate)
        {
            UpdateContents();
            refreshCounter = 0;
        }
    }

    private List<BuildingType> permittedBuildingTypes = new List<BuildingType>();
    private List<BuildingType> cachedPermittedBuildingTypes = new List<BuildingType>();
    private void UpdateContents()
    {
        List<BuildingType> availableBuildingTypes = BuildingManager.Instance.GetAvailableBuildingTypes();
        
        cachedPermittedBuildingTypes.Clear();
        foreach(BuildingType bType in permittedBuildingTypes)
        {
            cachedPermittedBuildingTypes.Add(bType);
        }
        permittedBuildingTypes.Clear();
        if(currentActorUnit != null)
        {
            for(int i = 0; i < availableBuildingTypes.Count; i++)
            {
                BuildingType bType = availableBuildingTypes[i];
                if (currentActorUnit.Stats.Stats.HasAtLeast(bType.BuildingPrefab.StatRequirements))//perhaps replace this with just "can build"?  i guess stat req is ok.
                {
                    permittedBuildingTypes.Add(bType);
                }
            }
            foreach(BuildingType bType in cachedPermittedBuildingTypes)
            {
                if(!permittedBuildingTypes.Contains(bType))
                {
                    //removebutton(bType)
                }
            }
            foreach(BuildingType bType in permittedBuildingTypes)
            {
                if(!cachedPermittedBuildingTypes.Contains(bType))
                {
                    AddButton(bType);
                }
            }
        }
    }

    private void ClearButtons()
    {
        foreach(BuildBuildingButton button in childButtons)
        {
            buildBuildingButtonPool.RecycleObject(button.gameObject);
        }
        childButtons.Clear();
    }

    private void AddButtons(List<BuildingType> buildingTypes)
    {
        for (int i = 0; i < buildingTypes.Count; i++)
        {
            BuildingType bType = buildingTypes[i];
            BuildBuildingButton button = AddButton(bType);
            button.transform.SetSiblingIndex(i);
        }
    }

    private BuildBuildingButton AddButton(BuildingType bType)
    {
        BuildBuildingButton button = buildBuildingButtonPool.GetGameObject().GetComponent<BuildBuildingButton>();
        button.BType = bType;
        button.ActorUnit = currentActorUnit;
        button.Text.text = bType.BuildingPrefab.BuildingName;
        button.gameObject.SetActive(true);
        childButtons.Add(button);
        return button;
    }

}
