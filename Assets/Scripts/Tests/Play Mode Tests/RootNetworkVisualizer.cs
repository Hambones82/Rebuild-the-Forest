using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootNetworkVisualizer : MonoBehaviour
{
    private RootNetworkComponent _rnc;
    private TMPro.TextMeshPro text;

    private void Awake()
    {
        _rnc = GetComponent<RootNetworkComponent>();
        text = GetComponent<TMPro.TextMeshPro>();
    }
    
    float timer = 0;
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= UITuning.refreshRate)
        {
            //update shown value
            timer = 0;
            text.text = _rnc.NetworkID.ToString();
        }
    }
}
