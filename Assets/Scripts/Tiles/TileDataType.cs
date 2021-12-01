using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public enum TileDataType
{
    terrain,
    encroachment,
    pollution,
    debug_BuildingRangeOverlay,
    debug_general
}

public static class TileDataTypes
{
    public static readonly Dictionary<TileDataType, Type> tileDataLookup = new Dictionary<TileDataType, Type>
    {
        {TileDataType.terrain, typeof(TerrainTile) },
        {TileDataType.encroachment, typeof(EncroachmentTile) },
        {TileDataType.debug_BuildingRangeOverlay, typeof(Tile) }, //just the base tile -- this is just a graphical layer
        {TileDataType.debug_general, typeof(Tile) }
    };
}