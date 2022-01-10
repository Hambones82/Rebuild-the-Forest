using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RootManager : MonoBehaviour
{
    [SerializeField]
    private RootManager _instance;
    public RootManager Instance { get => _instance; }

    private List<Root> roots = new List<Root>();

    [SerializeField]
    private Root rootPrefab;

    private void Awake()
    {
        if (_instance != null) throw new InvalidOperationException("can't have two root managers");
    }

    void Update()
    {
        
    }
}
