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
    private StatLineSet statRequirements;
    public StatLineSet StatRequirements { get => statRequirements; }
}
