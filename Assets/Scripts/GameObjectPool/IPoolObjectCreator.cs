using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObjectCreator
{
    GameObject CreateNewPoolObject(GameObject parent = null, bool active = true);
}
