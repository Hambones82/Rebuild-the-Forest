using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    private GameObject _content;
    private Inventory selectedInventory;


    private void Awake()
    {
        UIManager.Instance.OnSelectEvent.AddListener(GTSelected);
        UIManager.Instance.OnDeselectEvent.AddListener(GTDeselected);
    }

    private void GTSelected()
    {

    }

    private void GTDeselected()
    {

    }

    private void UpdateContents()
    {

    }
}
