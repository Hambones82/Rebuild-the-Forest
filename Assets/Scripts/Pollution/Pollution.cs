using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Linq;

public class Pollution : MonoBehaviour
{
    [SerializeField]
    private PollutionManager pollutionManager;
    public PollutionManager PollutionManager { get => pollutionManager; set => pollutionManager = value; }

    private PollutionTypeController pTypeController;
    public PollutionTypeController PTypeController { get => pTypeController; set => pTypeController = value; }

    [SerializeField]
    private PollutionData _pollutionData;
    public PollutionData PollutionData { get => _pollutionData; }

    public int Priority { get => _pollutionData.Priority; }

    [SerializeField]
    private List<PollutionEffect> innatePollutionEffects;
    public IReadOnlyList<PollutionEffect> PollutionEffects 
    {
        get
        {
            List <PollutionEffect> retval = new List<PollutionEffect>(innatePollutionEffects);
            //if pollutionManager != null is a hack and i'm not sure how to fix it...
            if(pollutionManager != null)
            {
                retval.AddRange(pTypeController.GetSourceEffectsAt(GetComponent<GridTransform>().topLeftPosMap));
            }            
            return retval;
        }
    }

    [SerializeField]
    private float maxAmount = 100;
    public float MaxAmount { get => maxAmount; }
    [SerializeField]
    private float amount;
    public float Amount { get => amount; }

    [SerializeField]
    private List<MapEffectType> cleanEnableEffects;
    public List<MapEffectType> CleanEnableEffects { get => new List<MapEffectType>(cleanEnableEffects); }

    public UnityEvent OnDisableEvent;

    //just have a pollution.IsCleanable, use an effect...

    public bool IsCleanable(Vector2Int cell)
    {        
        foreach(var cleanableEffect in PollutionEffects.OfType<IPollutionCleanable>())
        {            
            if(!cleanableEffect.IsCleanable(cell))
            {
                return false;
            }            
        }
        return true;
    }

    public void OnClean()
    {
        foreach(var cleanableEffect in PollutionEffects.OfType<IPollutionCleanable>())
        {    
            cleanableEffect.OnClean(GetComponent<GridTransform>().topLeftPosMap); //COME BACK HERE!!!            
        }
    }

    //this thing...  i'm not sure it makes sense to be a 
    public bool IsSpawnBlocked(Vector2Int cell)
    {        
        foreach(var blocker in PollutionEffects.OfType<IPollutionSpawnBlock>())
        {            
            if(blocker.BlocksPollutionGrowth(cell))
            {
                return true;
            }            
        }
        return false;
    }

    public void OnSpawnBlocked(Vector2Int cell)
    {        
        foreach (var blocker in PollutionEffects.OfType<IPollutionSpawnBlock>())
        {
            blocker.OnSpawnBlocked(cell);
        }        
    }

    private void Awake()
    {        
        SetAmount(maxAmount);        
    }

    private void OnEnable()
    {
        //maybe don't do this?
        /*
        foreach(IPollutionOnSpawn effect in PollutionEffects.OfType<IPollutionOnSpawn>())
        {
            effect.OnSpawn(this);
        }*/
    }

    private void Update()
    {
    }

    //returns whether the pollution was removed
    public bool SetAmount(float amt)
    {
        amount = Mathf.Clamp(amt, 0, maxAmount);
        float scale = amt / maxAmount;
        transform.localScale = new Vector3(scale, scale);
        if (amount == 0)
        {
            OnClean();
            pollutionManager.RemovePollution(this);
            return true;
        }
        return false;
    }
    
    private void OnDisable()
    {
        if(OnDisableEvent != null)
        {
            OnDisableEvent.Invoke();
            OnDisableEvent.RemoveAllListeners();
        }
        /*
        foreach (IPollutionOnDeath effect in PollutionEffects.OfType<IPollutionOnDeath>())
        {
            effect.OnDeath(this);
        }*/
    }

    //effect onspawn and effect ondeath should also be called when the pollution effects get removed or added to the pollution
}
