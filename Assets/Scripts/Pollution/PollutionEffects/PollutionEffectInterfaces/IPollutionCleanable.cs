using UnityEngine;


public interface IPollutionCleanable
{
    public bool IsCleanable(Vector2Int cell);
    public void OnClean(Vector2Int cell);
}
