using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//delete
[System.Serializable]
public class MapEffect 
{
    public delegate void OnChangeEnabledDelegate(MapEffect effect);
    public event OnChangeEnabledDelegate OnChangeEnabled;

    [SerializeField]
    private int _range;
    public int Range { get => _range; }

    [SerializeField]
    private bool _enabled;
    public bool Enabled { get => _enabled; set => OnChangeEnabled(this); }

    [SerializeField]
    private MapEffectObject _effect;
    public MapEffectObject Effect { get => _effect; }
}
