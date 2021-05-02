using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypedNumericalComparable<DATA> : TypedComparable<DATA, float> 
{
    public TypedNumericalComparable(DATA d, float f) : base(d, f) { }
}
