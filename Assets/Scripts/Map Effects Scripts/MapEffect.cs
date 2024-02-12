using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapEffect 
{
    //alright, so all of the...  commented out stuff can be removed...
    //why do i have this?  remove?
    /*
    public delegate void OnChangeEnabledDelegate(MapEffect effect);
    public event OnChangeEnabledDelegate OnChangeEnabled;
    */

    [SerializeField]
    private int _range;
    public int Range { get => _range; }

    
    [SerializeField]
    private bool _enabled = true;
    public bool Enabled { get => _enabled; set => _enabled = value; }
    

    [SerializeField]
    private MapEffectObject _effect;
    public MapEffectObject Effect { get => _effect; }
}
