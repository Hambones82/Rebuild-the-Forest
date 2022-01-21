using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PrefabReferences : MonoBehaviour
{
    private static PrefabReferences _instance;
    public static PrefabReferences Instance { get => _instance; }

    private void Awake()
    {
        if (_instance != null) throw new InvalidOperationException("cannot make two prefabreferences");
        else _instance = this;
        
    }
    
    [SerializeField]
    private RootCursor _rootCursor;
    public RootCursor RootCursor { get => _rootCursor; }
}
