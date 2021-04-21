using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TerrainTile", menuName = "Tiles/Terrain Tile")]
public class TerrainTile : Tile
{
    [SerializeField]
    private bool buildable;
    public bool Buildable
    {
        get => buildable;
    }
}


//TileProperty...  tile data has a list of tileproperties.  you can check for specific properties.  is this what i even want?
//if this is an abstract class, aren't we just defining the individual tile things via inheritance?
//what do i want to look for?  


//ok, the purpose of tile data is... to allow the gridmap to provide you with information about the tiles
//in addition, the reason we might want sub-classing is to allow different types of tiles (e.g., terrain, others), to have different types
//of tiles / data...  so... 

//but ok what's the best way to do this.  in other words...  

//i feel like... this tile property thing should be... used elsewhere...

//why am i even making this thing a separate class as compared with terraintile?  
//i think the idea is that... our building checker... wants to get terrain tile data... so it needs to request that data.  
//but why can't it just do that directly?  
//let's just have a function in gridmap -- get terrain tile data... that should be ok.
//on the tile data, just have public properties with getter only.