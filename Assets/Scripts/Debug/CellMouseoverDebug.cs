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
        rectTransform.anchoredPosition = mouseManager.MouseScreenPosition;
        Vector2Int cell = mouseManager.GetMouseMapCoords();
        if(!gridMap.IsWithinBounds(cell))
        {
            text.text = "Out of map area.";
        }
        else if(!gridMap.IsCellOccupied(cell))
        {
            text.text = "Empty cell";
        }
        else
        {
            List<GridTransform> objects = gridMap.GetObjectsAtCell(cell);
            text.text = $"num objects: {objects.Count}";
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
    }
}
