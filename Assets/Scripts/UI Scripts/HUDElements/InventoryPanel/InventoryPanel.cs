using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : ScrollableContentPanel
{
    private Inventory _inventory;

    protected override void UpdateContents()
    {
        if(_inventory != null )
        {
            ClearButtons();
            foreach (InventoryItem item in _inventory.GetAllInventoryItems())
            {
                AddButton(item);
            }
        }
    }

    private void AddButton(InventoryItem item)
    {
        GameObject newItem = contentObjectPool.GetGameObject();
        contentObjects.Add(newItem);
        newItem.transform.SetAsLastSibling();
        newItem.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = item.ItemType.InventoryImage;
        newItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Amount.ToString() + "x";
        newItem.SetActive(true);
    }

    protected override void ProcessSelectionEvent()
    {
        base.ProcessSelectionEvent();
    }

    protected override void SetCachedReferences()
    {
        base.SetCachedReferences();
        _inventory = currentActorUnit.GetComponent<Inventory>();
    }

    protected override void ProcessDeselectionEvent()
    {
        base.ProcessDeselectionEvent();
        _inventory = null;
    }
}
