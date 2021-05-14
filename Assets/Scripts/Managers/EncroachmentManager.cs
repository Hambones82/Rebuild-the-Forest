using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EncroachmentManager : MonoBehaviour
{
    private readonly int externalPadding = 3;

    private int topRow;
    private int bottomRow;
    private int leftColumn;
    private int rightColumn;
    private int paddedHeight;
    private int paddedWidth;

    private float topEncroachment;
    private float bottomEncroachment;
    private float leftEncroachment;
    private float rightEncroachment;

    [SerializeField]
    private float encroachmentRate;

    [SerializeField]
#pragma warning disable CS0649 // Field 'EncroachmentManager.encroachmentTileMap' is never assigned to, and will always have its default value null
    private TileDataMap encroachmentTileMap;
#pragma warning restore CS0649 // Field 'EncroachmentManager.encroachmentTileMap' is never assigned to, and will always have its default value null

    [SerializeField]
#pragma warning disable CS0649 // Field 'EncroachmentManager.gridMap' is never assigned to, and will always have its default value null
    private GridMap gridMap;
#pragma warning restore CS0649 // Field 'EncroachmentManager.gridMap' is never assigned to, and will always have its default value null

    [SerializeField] //the order of tiles in the list 
#pragma warning disable CS0649 // Field 'EncroachmentManager.encroachmentTiles' is never assigned to, and will always have its default value null
    private List<EncroachmentTile> encroachmentTiles;//the different types of tiles... though is this even necessary???
#pragma warning restore CS0649 // Field 'EncroachmentManager.encroachmentTiles' is never assigned to, and will always have its default value null

    public void Awake()
    {
        Initialize();
    }
    //(0,0) is bottom left
    private void Initialize()
    {
        if(encroachmentTiles[0] == null)
        {
            throw new InvalidOperationException("the encroachment manager has no encroachment tiles set, but must have at least one");
        }
        EncroachmentTile tile = encroachmentTiles[0];
        //set up the tilemap
        topRow = gridMap.height + externalPadding - 1;
        bottomRow = -1 * externalPadding;
        leftColumn = -1 * externalPadding;
        rightColumn = gridMap.width + externalPadding - 1;
        paddedHeight = topRow - bottomRow + 1;
        paddedWidth = rightColumn - leftColumn + 1;

        encroachmentTileMap.SetSize(new Vector2Int(paddedWidth, paddedHeight));
        //set the tiles in those rows and columns...
        FillRow(tile, topRow);
        FillRow(tile, bottomRow);
        FillColumn(tile, leftColumn);
        FillColumn(tile, rightColumn);
        Vector2Int coord1 = new Vector2Int(0, bottomRow);
        Vector2Int coord2 = new Vector2Int(0, topRow);
    }

    private void FillRow(EncroachmentTile tile, int row)
    {
        Vector2Int coords = new Vector2Int(0, row);
        for (coords.x = leftColumn; coords.x <= rightColumn; coords.x++)
        {
            encroachmentTileMap.SetTileAt(tile, coords);
        }
    }

    private void FillColumn(EncroachmentTile tile, int column)
    {
        Vector2Int coords = new Vector2Int(column, 0);
        for(coords.y = bottomRow; coords.y <= topRow; coords.y++)
        {
            encroachmentTileMap.SetTileAt(tile, coords);
        }
    }

    public void EndTurn()
    {
        //ok, here's the algorithm i'm thinking.
        //from each direction - top, bottom, left, right, an invisible "front" proceeds at a fixed speed.
        //anything behind the front is encroached, anything in front is unencroached.
        //so is the idea that... your protection... holds up the front?  or does it just...  
        //ok... let's just do this for now.
    }
}
