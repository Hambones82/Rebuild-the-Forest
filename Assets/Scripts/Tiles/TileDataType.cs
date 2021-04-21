using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public enum TileDataType
{
    terrain,
    encroachment,
    pollution,
    debug_BuildingRangeOverlay
}

public static class TileDataTypes
{
    public static readonly Dictionary<TileDataType, Type> tileDataLookup = new Dictionary<TileDataType, Type>
    {
        {TileDataType.terrain, typeof(TerrainTile) },
        {TileDataType.encroachment, typeof(EncroachmentTile) },
        {TileDataType.pollution, typeof(PollutionTile) },
        {TileDataType.debug_BuildingRangeOverlay, typeof(Tile) } //just the base tile -- this is just a graphical layer
    };
}