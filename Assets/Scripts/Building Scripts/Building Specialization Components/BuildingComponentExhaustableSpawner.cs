using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponentExhaustableSpawner : BuildingComponentOperator
{
    [SerializeField]
    private float _operateTimer = 0;

    [SerializeField]
    private float _operateTimerTrigger;

    [SerializeField]
    private bool _exhausted = false;

    public override bool Operate(float dt)
    {
        if (_exhausted) return false;
        _operateTimer += dt;
        if(_operateTimer >= _operateTimerTrigger)
        {
            Trigger();
            return false;
        }
        return true;
    }

    private void Trigger()
    {
        if (!ActorUnitManager.Instance.ActorUnitsFull)
        {
            Vector2Int cellToSpawnAt;
            List<Vector2Int> candidateCells = GetComponent<GridTransform>().GetAdjacentTiles();
            cellToSpawnAt = candidateCells[Random.Range(0, candidateCells.Count)];
            ActorUnitManager.Instance.SpawnActorUnit(cellToSpawnAt);
            _exhausted = true;
        }
    }

    public override bool IsOperatable()
    {
        return !_exhausted;
    }
}
