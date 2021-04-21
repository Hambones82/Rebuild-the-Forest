using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHUD : MonoBehaviour
{
    public PlayerResourceData playerData;
    public ResourceHUDElement HUDElementPrefab;

    private GameObjectPool HUDElementPool;
    //object pool?
    private List<ResourceHUDElement> resourceHUDElements = new List<ResourceHUDElement>();

    public void Awake()
    {
        HUDElementPool = new GameObjectPool(HUDElementPrefab.gameObject);
        UpdateHUDElements();
        playerData.OnBuildingsChanged.AddListener(UpdateHUDElements);
    }

    public void Update()
    {
        
    }

    public void UpdateHUDElements()
    {
        ClearAll();
        for (int i = 0; i < playerData.numDisplayResources; i++)
        {
            ResourceHUDElement nextElement = HUDElementPool.GetGameObject().GetComponent<ResourceHUDElement>();
            resourceHUDElements.Add(nextElement);
            nextElement.SetLabel(playerData.displayResourceTypes[i].ToString());
            nextElement.SetValue(playerData.displayResourceAmounts[i].ToString());
            nextElement.transform.SetParent(this.gameObject.transform);
            nextElement.gameObject.SetActive(true);
        }
    }

    private void ClearAll()
    {
        foreach(ResourceHUDElement rhe in resourceHUDElements)
        {
            HUDElementPool.RecycleObject(rhe.gameObject);
        }
        resourceHUDElements.Clear();
    }
}
