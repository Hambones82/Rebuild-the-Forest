using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySceneLoader : MonoBehaviour
{
    [SerializeField] GameObject world;    
    [SerializeField] GameObject UIPreFab;
    [SerializeField] List<MonoBehaviour> gameManagers;
    [SerializeField] ServiceLocator _serviceLocator = new ServiceLocator();
    [SerializeField] List<InputDefinitionModuleSO> inputDefinitionModuleSOs;
    private void Awake()
    {
        //GameObject.Instantiate(world);             
        
        GameObject managers = new GameObject("Managers");
        List<GameObject> createdManagers = new List<GameObject>();
        
        foreach (var manager in gameManagers)
        {
            GameObject newManager = Object.Instantiate(manager, managers.transform).gameObject;
            createdManagers.Add(newManager);
            IGameManager gameManager = newManager.GetComponent<IGameManager>();
            if(gameManager != null)
            {
                gameManager.SelfInit(_serviceLocator);
            }            
        }
        foreach(var manager in createdManagers)
        {
            IGameManager iGameManager = manager.GetComponent<IGameManager>();
            if(iGameManager != null)
            {
                iGameManager.MutualInit();
            }
        } 
        
        foreach(var inputDefinition in inputDefinitionModuleSOs)
        {
            inputDefinition.Initialize(_serviceLocator);
        }
        //also need to initialize all the input modules.

        GameObject.Instantiate(UIPreFab);
    }
}
