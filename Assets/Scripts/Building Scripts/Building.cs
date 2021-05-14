using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Building : MonoBehaviour
{
    [SerializeField]
    private BuildingManager buildingManager;
    public BuildingManager BuildingManager { get => buildingManager; set => buildingManager = value; }

    [SerializeField]
#pragma warning disable CS0649 // Field 'Building.statRequirements' is never assigned to, and will always have its default value null
    private StatLineSet statRequirements;
#pragma warning restore CS0649 // Field 'Building.statRequirements' is never assigned to, and will always have its default value null
    public StatLineSet StatRequirements { get => statRequirements; }
}
