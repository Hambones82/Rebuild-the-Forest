using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentExhaustableSpawner : BuildingComponentOperator
{
    
    [SerializeField]
    private bool _exhausted = false;    

    [SerializeField]
    private Sprite exhaustedSprite;   

    protected override void Trigger(GameObject gameObject = null)
    {        
        TriggerForActorHarvest();        
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
        return false;
    }
}
