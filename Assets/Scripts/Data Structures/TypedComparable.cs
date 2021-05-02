using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class TypedComparable<TYPE, DATA> where DATA : IComparable
{
    [SerializeField]
    protected TYPE type;
    public TYPE Type { get => type; }
    [SerializeField]
    protected DATA data;
    public DATA Data { get => data; }

    public TypedComparable(TYPE t, DATA d)
    {
        type = t;
        data = d;
    }
}
