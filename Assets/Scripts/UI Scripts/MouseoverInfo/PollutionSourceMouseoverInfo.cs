using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionSourceMouseoverInfo : MouseoverInfo
{
    public override string Text 
    { 
        get
        {
            return $"Pollution Source, part of group {PollutionManager.Instance.GetPSourceGroupID(GetComponent<PollutionSource>())}";
        }
    }
}
