using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingInitDataItem
{
    [SerializeField] BuildingType buildingType;
    public BuildingType BuildingType => buildingType;
    [SerializeField] int count;
    public int Count => count;
}

[System.Serializable]
public class BuildingInitData
{
    [SerializeField] List<BuildingInitDataItem> initData;
    public IEnumerable<(BuildingType buildingType, int count)> InitDataItems
    {
        get
        {
            foreach (var item in initData)
            {
                yield return (buildingType: item.BuildingType , count: item.Count);
            }
        }
    }    
}
