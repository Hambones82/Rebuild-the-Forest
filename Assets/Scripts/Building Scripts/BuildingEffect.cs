using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingEffect 
{
    [SerializeField]
    private int _range;
    public int Range { get => _range; }

    [SerializeField]
    private MapEffect _effect;
    public MapEffect Effect { get => _effect; }
}
