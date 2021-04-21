using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using System;

// so this appears to display the minimap.

//transform should just be based on the map coordinates, not the grid coordinates... important.
//this is messed up... number of cells in width and height is not how it's described and this really needs to be fixed.

    //execution order?
public class GUIMapDisplayer : MonoBehaviour {
    public Color occupiedColor;
    public Color emptyColor;
    public Color paddingColor;

    public GridMap gridMap; //the object to which the map we are going to display is attached
    private GridSubMap gridSubMap; //the map we are going to display

    public RawImage rawImage;

    private Texture2D mapTexture;
    private Color[] mapData;
    private Vector2Int mapSize;

    public MapLayer mapLayer;
    // Use this for initialization
    //see if i can just use independent texture instead of the texture coupled to the same gameobject on a raw image...
    void Start () {
        gridSubMap = gridMap.GetMapOfType(mapLayer);//?????probably bc deleted those guys and now no gridmap is created...
        
        Assert.IsTrue(gridSubMap != null);
        rawImage = GetComponent<RawImage>();
        mapTexture = rawImage.GetComponent<Texture2D>();
        mapSize = gridSubMap.GetSize();
        int mapSizeLargestDimension = (mapSize.x > mapSize.y ? mapSize.x : mapSize.y);
        mapData = new Color[mapSizeLargestDimension * mapSizeLargestDimension];
        if (mapTexture == null)
        {
            rawImage.texture = new Texture2D(mapSizeLargestDimension,mapSizeLargestDimension);
            mapTexture = rawImage.texture as Texture2D;
            mapTexture.filterMode = FilterMode.Point;
        }
        gridSubMap.mapChangeEvent += UpdateMapTexture;
        
        UpdateMapTexture();
        
	}
	
	// Update is called once per frame
	void Update () {
        //UpdateMapTexture();  // want to do this sometimes... but not every frame.  gotta figure out how to do this on a delay
	}

    private void UpdateMapTexture()
    {
        //need to make it square... i'd add padding on one side or the other...
        if(mapTexture != null)
        {
            Vector2Int mapSize = gridSubMap.GetSize();
            int index = 0;
            if (mapSize.x > mapSize.y)//pad horizontally
            {
                //the math is that we subtract y from x and divide the result by 2.  then we do rows 0 to result-1 and result + y to x-1
                int padHeightDiv2 = (mapSize.x - mapSize.y) / 2;
                for(int i = 0; i < padHeightDiv2*mapSize.x; i++)
                {
                    mapData[i] = paddingColor;
                }
                for(int i = (padHeightDiv2) * mapSize.x; i < mapSize.x*mapSize.x; i++)
                {
                    mapData[i] = paddingColor;
                }

                index = padHeightDiv2 * mapSize.x;
            }
            else if (mapSize.x < mapSize.y) // pad vertically
            {

            }

            
            
            for (int j = 0; j < mapSize.y; j++)
            {
                for (int i = 0; i < mapSize.x; i++)
                {
                    SetColor(index, gridSubMap.IsCellOccupied(new Vector2Int(i, j)), gridSubMap.GetMinimapColor(new Vector2Int(i, j)));
                    index++;
                }

            }
            mapTexture.SetPixels(mapData);
        }
        mapTexture.Apply();
    }

    private void SetColor(int index, bool occupied, Color inputColor)
    {
        
        if(occupied)
        {
            mapData[index] = inputColor;
        }
        else
        {
            mapData[index] = emptyColor;
        }
    }
}
