using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfigData", menuName = "ScriptableObjects/ConfigData/Camera Config")]

//also need to do the create asset
public class CameraConfigData : ScriptableObject
{
    public float padding; // number of world units that the center pads up to when reaches the border
    public float moveSpeed; //how fast the camera moves
    public float zoomSpeed;
    public float maxZoom;
    public float minZoom;
}
