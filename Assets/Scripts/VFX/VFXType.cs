using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXType", menuName = "ScriptableObjects/Types/VFX")]
public class VFXType : ObjectType<VFX>
{
    [SerializeField]
    private VFX vfxPrefab;
    public VFX VFXPrefab { get => vfxPrefab; }
    public override VFX GetObject()
    {
        return vfxPrefab;
    }
}
