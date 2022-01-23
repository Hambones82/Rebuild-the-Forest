using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    public Direction connectivity;
    [SerializeField]
    private int _rootNetwork;
    public int RootNetwork
    {
        get => _rootNetwork;
        set => _rootNetwork = value;
    }
}
