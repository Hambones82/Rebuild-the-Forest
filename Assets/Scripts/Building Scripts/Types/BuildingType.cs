using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingType", menuName = "ScriptableObjects/Types/Building")]
public class BuildingType : ObjectType<Building>
{
    [SerializeField]
    private Building buildingPrefab;
    public Building BuildingPrefab { get => buildingPrefab; }
    public override Building GetObject()
    {
        return buildingPrefab;
    }
}
