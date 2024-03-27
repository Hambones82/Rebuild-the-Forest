using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionMouseOverInfo : MouseoverInfo
{
    public override string Text
    {
        get
        {
            //the group id
            Vector2Int pos = GetComponent<GridTransform>().topLeftPosMap;
            string retval = "";
            retval += $"pollution group: {PollutionManager.Instance.GetGraphID(pos)}";
            return retval;
        }
    }
}
