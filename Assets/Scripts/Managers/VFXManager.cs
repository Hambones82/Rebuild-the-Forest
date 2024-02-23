using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public void SpawnEffect(VFXType vfxType, Vector3 position)
    {
        Instantiate(vfxType.VFXPrefab, position, Quaternion.identity);
    }
}
