
using UnityEngine;

public interface IPollutionSpawnBlock
{
    public bool BlocksPollutionGrowth(Vector2Int cell);
    public void OnSpawnBlocked(Vector2Int cell);
}
