using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Building : MonoBehaviour
{
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
    private List<BuildingEffect> _buildingEffects;
    public List<BuildingEffect> BuildingEffects { get { return new List<BuildingEffect>(_buildingEffects); } }
}
