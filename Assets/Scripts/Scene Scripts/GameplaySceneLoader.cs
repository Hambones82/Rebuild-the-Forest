using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySceneLoader : MonoBehaviour
{
    [SerializeField] GameObject world;
    [SerializeField] GameObject managersPrefab;
    [SerializeField] GameObject UIPreFab;

    private void Awake()
    {
        GameObject.Instantiate(world);
        GameObject.Instantiate(managersPrefab);        
        GameObject.Instantiate(UIPreFab);
        //i think we gotta just use this to inject all the things...
        //set them all to something?
        //spawn the actual game entities?  all the maps, etc?
    }
}
