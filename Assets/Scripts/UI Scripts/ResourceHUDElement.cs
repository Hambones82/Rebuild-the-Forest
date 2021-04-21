using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceHUDElement : MonoBehaviour, IPoolObjectRecycler
{
    public GameObject label;
    public GameObject value;

    public void RecycleObject()
    {
        label.GetComponent<TextMeshProUGUI>().text = "";
        value.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void SetLabel(string lbl)
    {
        label.GetComponent<TextMeshProUGUI>().text = lbl;
    }

    public void SetValue(string valu)
    {
        value.GetComponent<TextMeshProUGUI>().text = valu;
    }
}
