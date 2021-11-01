using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellMouseoverDebug : MonoBehaviour
{
    [SerializeField]
    MouseManager mouseManager;
    [SerializeField]
    GridMap gridMap;
    GameObject mouseoverDebugPanel;
    TMPro.TextMeshProUGUI text;
    RectTransform rectTransform;


    private void Awake()
    {
        text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }
    void Update()
    {
        text.text = "";
        rectTransform.anchoredPosition = mouseManager.MouseScreenPosition;
        Vector2Int cell = mouseManager.GetMouseMapCoords();
        text.text += $"cell coords: {cell.ToString()} \n";
        if(!gridMap.IsWithinBounds(cell))
        {
            text.text += "Out of map area.";
            return;
        }
        if(!gridMap.IsCellOccupied(cell))
        {
            text.text += "Empty cell";
        }
        else
        {
            List<GridTransform> objects = gridMap.GetObjectsAtCell(cell);
            text.text += $"num objects: {objects.Count}";
            foreach (GridTransform gt in objects)
            {
                MouseoverInfo mouseoverInfo = gt.GetComponent<MouseoverInfo>();
                if(mouseoverInfo != null)
                {
                    text.text += "\n" + mouseoverInfo.Text;
                }
                else
                {
                    text.text += "\n" + gt.mapLayer.ToString();
                }
                
            }
        }
        TerrainTile tile = (TerrainTile)gridMap.GetTileAt(typeof(TerrainTile), cell);
        text.text += "\n" + "Buildable?: " + tile.Buildable.ToString();
        List<MapEffect> effects = MapEffectsManager.Instance.GetEffectsAtCell(cell);
        if (effects != null)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                text.text += $"\nEffect {i}: " + effects[i].ToString();
            }
        }
    }
}
