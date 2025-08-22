using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DiscoverableManager : MonoBehaviour, IGameManager
{
    public delegate void DiscoverableChangeDelegate(int x, int y, bool b);
    public event DiscoverableChangeDelegate OnDiscoverableChange;

    private bool[,] _discovered; //don't set this directly -- doing so fails to call callbacks
    private int width, height;

    [SerializeField] private GridMap _gridMap;

    [SerializeField]
    private DropTable pollutionDropTable;

    public void SetDiscovered(int x, int y, bool b)
    {
        if(x < 0 || y < 0 || x >= width || y >= height)
        {
            throw new ArgumentOutOfRangeException("Trying to set discovered pollution value out of bounds of map");
        }
        else
        {
            _discovered[x, y] = b;
            OnDiscoverableChange?.Invoke(x, y, b);
        }
    }

    private PollutionManager _pollutionManager;
    private ServiceLocator _serviceLocator;
    
    public void SelfInit(ServiceLocator serviceLocator)
    {
        if (serviceLocator == null) throw new ArgumentNullException("service locator cannot be null");
        _serviceLocator = serviceLocator;
        _serviceLocator.RegisterService(this);
        pollutionDropTable.Initialize();
    }
    public void MutualInit()
    {
        //gridmap should be given as reference
        width = _gridMap.width;
        height = _gridMap.height;
        _discovered = new bool[width, height];
        _pollutionManager = _serviceLocator.LocateService<PollutionManager>();
        _pollutionManager.OnInitComplete += InitializeDiscovered;
        _pollutionManager.OnPollutionDead += SpawnDiscovered;
    }

    private void InitializeDiscovered()
    {
        GridSubMap pollutionGridMap = GridMap.Current.GetMapOfType(MapLayer.pollution);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SetDiscovered(x,y, pollutionGridMap.GetObjectsAtCell(new Vector2Int(x,y)).Count == 0);
            }
        }
    }

    private void SpawnDiscovered(Vector2Int cell)
    {
        if(!_discovered[cell.x, cell.y])
        {
            BuildingType buildingType = pollutionDropTable.GetDroppedBuildingType();
            if(buildingType != null)
            {
                BuildingManager.Instance.SpawnBuildingAt(buildingType.BuildingPrefab, cell);
            }
            SetDiscovered(cell.x, cell.y, true);
        }
    }
}
