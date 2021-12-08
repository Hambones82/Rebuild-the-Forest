using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensionMethods 
{
    public static void SortChildren(this Transform transform, System.Comparison<Transform> comparison)
    {

        List<Transform> transforms = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++)
        {
            transforms.Add(transform.GetChild(i));
        }
        transforms.Sort(comparison);
        for(int i = 0; i < transform.childCount; i++)
        {
            transforms[i].SetSiblingIndex(i);
        }
    }
}
