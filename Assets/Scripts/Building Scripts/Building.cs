using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Building : MonoBehaviour
{
    public delegate void OnBuildingDeathEvent(Building building);
    public event OnBuildingDeathEvent onBuildingDeathEvent;

    [SerializeField]
    private string buildingName;
    public string BuildingName { get => buildingName; set => buildingName = value; }

    [SerializeField]
    private BuildingManager buildingManager;
    public BuildingManager BuildingManager { get => buildingManager; set => buildingManager = value; }

    [SerializeField]
    private StatLineSet statRequirements;
    public StatLineSet StatRequirements { get => statRequirements; }

    [SerializeField]
    private bool _killActorUnit;
    public bool KillActorUnit { get => _killActorUnit; }

    [SerializeField]
    private bool _itemRequiredToBuild;
    public bool ItemRequiredToBuild { get => _itemRequiredToBuild; }

    [SerializeField]
    private InventoryItemType _requiredItem;
    public InventoryItemType RequiredItem { get => _requiredItem; }
    
    private void OnDisable()
    {
        onBuildingDeathEvent?.Invoke(this);
    }
    /*
    [SerializeField]
    private List<BuildingEffect> _buildingEffects;
    public List<BuildingEffect> BuildingEffects { get { return new List<BuildingEffect>(_buildingEffects); } }
    */
}
