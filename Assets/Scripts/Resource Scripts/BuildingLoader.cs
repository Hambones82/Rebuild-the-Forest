using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLoader : PrefabLoader<BuildingType, BuildingLoader>
{
    private BuildingLoader()
    {
        foreach (KeyValuePair<BuildingType, string> kvp in _prefabPaths)
            loadedPrefabs.Add(kvp.Key, Resources.Load(kvp.Value) as GameObject);
    }

    private const string BuildingPath = "Prefabs/Buildings/";
    private static Dictionary<BuildingType, string> _prefabPaths = new Dictionary<BuildingType, string>()
    {
        { BuildingType.WoodFactory, BuildingPath + "WoodFactory"},
        {BuildingType.WoodYard, BuildingPath + "WoodYard" },
        {BuildingType.WoodRoad, BuildingPath + "WoodRoad" },
        {BuildingType.WoodMill, BuildingPath + "Wood Mill" },
        {BuildingType.Farm, BuildingPath + "Farm" },
        {BuildingType.House, BuildingPath + "House" },
        {BuildingType.Town, BuildingPath + "GrowableTown" }
    };

    protected override Dictionary<BuildingType, string> prefabPaths
    {
        get
        {
            return _prefabPaths;
        }
    }
}
