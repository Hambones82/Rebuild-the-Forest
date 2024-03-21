using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SplotchParameters
{
    public uint width;
    public uint height;
    public uint numCellsHorizontal;
    public uint numCellsVertical;
    public uint minSplotchRadius;
    public uint maxSplotchRadius;
    public uint borderWidth;
    public uint seed; //0 to int max?
    public float splotchProbability;
}

public static class SplotchGenerator 
{
    private static uint AdvanceSeed(uint seed)
    {
        const uint a = 1664525; 
        const uint c = 1013904223; 

        return (uint) (a * seed + c);
    }

    public static bool[,] GenerateSplotchMap(SplotchParameters parameters)
    {        
        // Initialize the bitmap
        bool[,] map = new bool[parameters.width, parameters.height];
        for (int i = 0; i < parameters.width; i++)
        {
            for (int j = 0; j < parameters.height; j++)
            {
                map[i, j] = false;
            }
        }
        bool[,] occupiedSplotchCells = new bool[parameters.numCellsHorizontal, parameters.numCellsVertical];
        for(int i = 0; i < parameters.numCellsHorizontal; i++)
        {
            for(int j = 0; j < parameters.numCellsVertical; j++)
            {
                occupiedSplotchCells[i, j] = false;
            }
        }
        uint seed = parameters.seed;

        for(int x = 0; x < parameters.numCellsHorizontal; x++)
        {
            for(int y = 0; y < parameters.numCellsVertical; y++)
            {
                seed = AdvanceSeed(seed);
                if((float)(seed % 1000)/1000f  <= parameters.splotchProbability && !occupiedSplotchCells[x,y])
                {
                    seed = AdvanceSeed(seed);
                    uint randomRadius = RandomRange(seed, parameters.minSplotchRadius, parameters.maxSplotchRadius);
                    uint cellWidth = parameters.width / parameters.numCellsHorizontal;
                    uint cellHeight = parameters.height / parameters.numCellsVertical;
                    Vector2Int topLeftCorner = new Vector2Int((int)(cellWidth * x), (int)(cellHeight * y));
                    Vector2Int bottomRightCorner = new Vector2Int((int)(cellWidth * (x+1)) - 1, (int)(cellHeight * (y+1)) - 1);
                    seed = AdvanceSeed(seed);
                    Vector2Int startPoint = RandomRange(seed, topLeftCorner, bottomRightCorner);             
                    bool[,] footprint = GenerateSplotchFooprint(randomRadius);
                    for(int footprintX = 0; footprintX < randomRadius * 2+1; footprintX++)
                    {
                        for(int footprintY = 0; footprintY < randomRadius * 2+1; footprintY++)
                        {
                            int xcoord = startPoint.x + footprintX;
                            int ycoord = startPoint.y + footprintY;
                            if(xcoord < parameters.width && ycoord < parameters.height)
                            {
                                bool pixel = footprint[footprintX, footprintY];                                
                                map[xcoord, ycoord] = pixel;                                
                            }
                        }
                    }
                }
            }
        }
        
        

        return map;
    }

    public static bool[,] GenerateSplotchFooprint(uint radius)
    {
        //Console.WriteLine($"radius: {radius}");
        bool[,] splotchFootprint = new bool[radius * 2+1, radius * 2+1];

        //if distance to center is less than radius, yes, otherwise, no
        Vector2Int center = new Vector2Int((int)(radius), (int)(radius));
        for(int x = 0; x < radius * 2+1; x++)
        {
            for(int y = 0; y < radius * 2+1; y++)
            {
                Vector2Int point = new Vector2Int(x, y);
                bool setPoint = GetDistance(point, center) <= radius ? true : false;                
                //if (setPoint) Console.WriteLine("pattern is being applied");
                splotchFootprint[x, y] = setPoint;
            }
        }
        return splotchFootprint;
    }

    private static double GetDistance(Vector2Int point1, Vector2Int point2)
    {
        int xDistance = Math.Abs(point2.x - point1.x);
        int yDistance = Math.Abs(point2.y - point1.y);
        return Math.Sqrt(xDistance*xDistance + yDistance*yDistance);

    }

    public static uint RandomRange(uint seed, uint min, uint max)
    {
        uint modulus = max - min + 1;        
        return (seed * 1534957) % modulus + min;
    }    

    public static Vector2Int RandomRange(uint seed, Vector2Int point1, Vector2Int point2)
    {
        int xmin = Math.Min(point1.x, point2.x);
        int xmax = Math.Max(point1.x, point2.x);
        int ymin = Math.Min(point1.y, point2.y);
        int ymax = Math.Max(point1.y, point2.y);

        uint randomX = RandomRange(seed, (uint)xmin, (uint)xmax);
        uint randomY = RandomRange(AdvanceSeed(seed), (uint)ymin, (uint)ymax);
        return new Vector2Int((int)randomX, (int)randomY);

    }
}

