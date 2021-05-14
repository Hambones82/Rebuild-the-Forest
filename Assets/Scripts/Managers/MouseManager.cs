using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class MouseManager : MonoBehaviour {

    public UIManager uIManager;
    
    public GridMap gridMap;
    private Plane TileMapPlane;
    [SerializeField]
    private Vector3 mouseWorldPosition = new Vector3(0, 0, 0);
    public Vector3 MouseWorldPosition { get => mouseWorldPosition; }
    [SerializeField]
    private Vector3 mouseScreenPosition = new Vector3(0, 0, 0);
    public Vector3 MouseScreenPosition { get => mouseScreenPosition; }

    [SerializeField]
    private Vector2Int mouseCellPos;
        
    public Vector2Int GetMouseMapCoords()
    {
        return gridMap.WorldToMap(GetMouseWorldCoords());
    }
    

    void Awake()
    {
        TileMapPlane = new Plane(Vector3.zero, Vector3.right, Vector3.up);//a hacky way to instantiate a plane at z=0
    }

    public Vector3 GetMouseWorldCoords()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, TileMapPlane.distance - Camera.main.gameObject.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        mouseCellPos = GetMouseMapCoords();
        mouseWorldPosition = GetMouseWorldCoords();
        if (Input.GetMouseButtonDown(0)) //if there has been a click,
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                uIManager.LeftClickAt(mouseWorldPosition);
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            if(!EventSystem.current.IsPointerOverGameObject())
            {
                uIManager.rightClickAt(mouseWorldPosition);
            }
        }
    }
}
