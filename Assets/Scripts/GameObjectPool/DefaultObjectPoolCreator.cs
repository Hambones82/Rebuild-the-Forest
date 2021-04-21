using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultObjectPoolCreator : IPoolObjectCreator
{
    private GameObject prefab;

   public DefaultObjectPoolCreator(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public GameObject CreateNewPoolObject(GameObject parent = null, bool active = true)
    {
        if(parent == null)
        {
            //Debug.Log("Instantiating without parent");
            GameObject newObj = GameObject.Instantiate(prefab) as GameObject;
            newObj.SetActive(active);
            return newObj;
        }
        else
        {
            //Debug.Log("Instantiating with parent");
            GameObject newObj = GameObject.Instantiate(prefab, parent.transform) as GameObject;
            newObj.SetActive(active);
            return newObj;
        }
    }
}
