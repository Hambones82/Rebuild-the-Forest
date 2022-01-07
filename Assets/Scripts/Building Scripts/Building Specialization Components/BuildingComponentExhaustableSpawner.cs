using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentExhaustableSpawner : BuildingComponentOperator
{
    
    [SerializeField]
    private bool _exhausted = false;

    [SerializeField]
    private bool _mushroomPlanted = false;

    [SerializeField]
    private Sprite exhaustedSprite;
    [SerializeField]
    private Sprite mushPlantedSprite;

    [SerializeField]
    private InventoryItemType cleanMushroomType;

    protected override void Trigger(GameObject gameObject = null)
    {
        if(_exhausted) //plant a mushroom
        {
            TriggerForMushroomGrowth(gameObject);
        }
        else //spawn the unit
        {
            TriggerForActorHarvest();
        }
        
    }

    private void TriggerForMushroomGrowth(GameObject inGameObject)
    {
        Inventory inventory = inGameObject.GetComponent<Inventory>();
        if(inventory.HasItem(cleanMushroomType))
        {
            inventory.RemoveItem(cleanMushroomType);
            _mushroomPlanted = true;
            GetComponent<SpriteRenderer>().sprite = mushPlantedSprite;
            GetComponent<ContinuousSpawner>().StartSpawning();
            //start the continuous spawner -- needs to be another component
        }
    }

    private void TriggerForActorHarvest()
    {
        if (!ActorUnitManager.Instance.ActorUnitsFull)
        {
            Vector2Int cellToSpawnAt;
            List<Vector2Int> candidateCells = GetComponent<GridTransform>().GetAdjacentTiles();
            cellToSpawnAt = candidateCells[Random.Range(0, candidateCells.Count)];
            ActorUnitManager.Instance.SpawnActorUnit(cellToSpawnAt);
            _exhausted = true;
            GetComponent<SpriteRenderer>().sprite = exhaustedSprite;
        }
    }

    public override bool IsOperatable(GameObject inGameObject)
    {
        if (!_exhausted) return true;
        else if (_mushroomPlanted) return false;
        else
        {
            Inventory inInventory = inGameObject.GetComponent<Inventory>();
            return inInventory.HasItem(cleanMushroomType);
        }
    }
}
