using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellMouseoverDebug : MonoBehaviour
{
    [SerializeField]
#pragma warning disable CS0649 // Field 'CellMouseoverDebug.mouseManager' is never assigned to, and will always have its default value null
    MouseManager mouseManager;
#pragma warning restore CS0649 // Field 'CellMouseoverDebug.mouseManager' is never assigned to, and will always have its default value null
    [SerializeField]
#pragma warning disable CS0649 // Field 'CellMouseoverDebug.gridMap' is never assigned to, and will always have its default value null
    GridMap gridMap;
#pragma warning restore CS0649 // Field 'CellMouseoverDebug.gridMap' is never assigned to, and will always have its default value null
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
