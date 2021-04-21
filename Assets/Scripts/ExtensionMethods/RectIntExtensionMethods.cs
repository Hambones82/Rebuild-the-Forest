using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class RectIntExtensionMethods {
    
    public static RectInt ClipRect(this RectInt rectToClip, RectInt clippingRect)
    {
        if((rectToClip.xMax < clippingRect.xMin )|| (rectToClip.yMax < clippingRect.xMin) || (rectToClip.xMin > clippingRect.xMax) || (rectToClip.yMin > clippingRect.yMax))
        {
            return new RectInt(0, 0, 0, 0);
        }
        else
        {
            RectInt retVal = new RectInt(rectToClip.position, rectToClip.size);
            retVal.ClampToBounds(clippingRect);
            return retVal;
        }
        
    }

    public static bool IsZero(this RectInt rectToTest)
    {
        if (rectToTest.size == Vector2Int.zero) return true;
        else return false;
    }


}
