using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//this is done in a very naive way, could be optimized... not necessary for prototype
public class PollutionManager : MonoBehaviour
{
    [SerializeField]
    private GridMap gridMap;
    [SerializeField]
    private GridSubMap pollutionMap;
    [SerializeField]
    private Pollution pollutionPrefab;
    [SerializeField]
    private List<Pollution> pollutionObjects = new List<Pollution>();

    [SerializeField]
    private MapEffectType pollutionBlockEffect;

    //this is a helper buffer to assist with another function.  maybe scope it to that function only
    private List<Pollution> workingPollutionObjects;

    [SerializeField, Range(0,1)]
    private float spreadBaseProbability = .01f;
    [SerializeField]
    private float spreadPeriod = .1f;//.1 seconds
    private float spreadTimer = 0;

    GameObjectPool pollutionPool;

    private void Awake()
    {
        pollutionPool = new GameObjectPool(pollutionPrefab.gameObject, parentObj: gridMap.gameObject, activeByDefault: false);
        pollutionMap = gridMap.GetMapOfType(MapLayer.pollution);
        
        
        foreach(GridTransform gt in pollutionMap.GetAllObjects())
        {
            Pollution pToAdd = gt.GetComponent<Pollution>();
            pToAdd.PollutionManager = this;
            pollutionObjects.Add(pToAdd);
        }
    }

    private void Update()
    {
        spreadTimer += Time.deltaTime;
        while(spreadTimer >= spreadPeriod)
        {
            spreadTimer -= spreadPeriod;
            UpdatePollutionState();
        }

    }

    private void UpdatePollutionState()
    {
        workingPollutionObjects = new List<Pollution>(pollutionObjects);
        foreach(Pollution pollution in workingPollutionObjects)
        {
            //Random.value
            Vector2Int pollutionPosition = pollution.GetComponent<GridTransform>().topLeftPosMap;
            float normalizedAmount = pollution.Amount / pollution.MaxAmount;
            UpdateCellPollution(new Vector2Int(pollutionPosition.x - 1, pollutionPosition.y), normalizedAmount);
            UpdateCellPollution(new Vector2Int(pollutionPosition.x + 1, pollutionPosition.y), normalizedAmount);
            UpdateCellPollution(new Vector2Int(pollutionPosition.x, pollutionPosition.y - 1), normalizedAmount);
            UpdateCellPollution(new Vector2Int(pollutionPosition.x, pollutionPosition.y + 1), normalizedAmount);
        }
    }

    private void UpdateCellPollution(Vector2Int cell, float normalizedAmount)
    {
        if(gridMap.IsWithinBounds(cell) && !pollutionMap.ComponentTypeExistsAtCell<Pollution>(cell))
        {
            bool addPollution = (CalculateProbabilityForSpreading(normalizedAmount)) > UnityEngine.Random.value;
            List<MapEffectObject> effectsAtCell = MapEffectsManager.Instance.GetEffectsAtCell(cell);
            if(effectsAtCell != null)
            {
                foreach (MapEffectObject effectObject in effectsAtCell)
                {
                    if (effectObject.EffectType == pollutionBlockEffect)
                    {
                        addPollution = false;
                        break;
                    }
                }
            }
            if(addPollution)
            {
                newPollution(cell);
            }
        }
    }

    private Pollution newPollution(Vector2Int cell)
    {
        GameObject newGO = pollutionPool.GetGameObject();
        newGO.SetActive(true);
        newGO.GetComponent<GridTransform>().MoveToMapCoords(cell);
        Pollution newPollution = newGO.GetComponent<Pollution>();
        pollutionObjects.Add(newPollution);
        newPollution.PollutionManager = this;
        newPollution.SetAmount(newPollution.MaxAmount);
        return newPollution;
    }

    public void RemovePollution(Pollution pollution)
    {
        pollutionObjects.Remove(pollution);
        pollutionPool.RecycleObject(pollution.gameObject);
    }

    private float CalculateProbabilityForSpreading(float normalizedAmount)//amount must be from 0 to 1.
    {
        if (normalizedAmount < 0 || normalizedAmount > 1)
            throw new InvalidOperationException("pollution spreading probability must be between 0 and 1");
        return normalizedAmount * spreadBaseProbability;
    }
}
