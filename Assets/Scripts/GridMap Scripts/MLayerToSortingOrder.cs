using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MLayerToSortingOrder
{
    public static readonly Dictionary<MapLayer, int> defs = new Dictionary<MapLayer, int>()
    {
        {MapLayer.buildings, 0 },
        {MapLayer.pollution, 1 },
        {MapLayer.roots, 2 },
        {MapLayer.playerUnits, 3 },
        {MapLayer.UI, 4 }
    };
}
