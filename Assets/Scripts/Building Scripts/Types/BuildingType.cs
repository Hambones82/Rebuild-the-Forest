using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingType : ObjectType<Building>
{
    [SerializeField]
#pragma warning disable CS0649 // Field 'BuildingType.buildingPrefab' is never assigned to, and will always have its default value null
    private Building buildingPrefab;
#pragma warning restore CS0649 // Field 'BuildingType.buildingPrefab' is never assigned to, and will always have its default value null
    public Building BuildingPrefab { get => buildingPrefab; }
}
