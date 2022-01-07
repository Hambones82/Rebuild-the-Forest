using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class ActorUnitDamageOverlay : MonoBehaviour
{
    private Image damageOverlayImage;

    private void Awake()
    {
        damageOverlayImage = GetComponent<Image>();
        DisableOverlay();
        ActorUnitManager.Instance.OnInitComplete += RegisterInitialActors;
    }

    private float time = 0;
    [SerializeField]
    private float overlayDuration;
    private bool overlayInOn;

    private void EnableOverlay()
    {
        damageOverlayImage.enabled = true;
        overlayInOn = true;
        time = 0;
    }

    private void DisableOverlay()
    {
        damageOverlayImage.enabled = false;
        overlayInOn = false;
    }

    private void Update()
    {
        if(overlayInOn)
        {
            time += Time.deltaTime;
            if(time >= overlayDuration)
            {
                DisableOverlay();
            }
        }
    }

    private void Start()
    {
        ActorUnitManager.Instance.OnActorUnitDeath -= DeregisterActor;
        ActorUnitManager.Instance.OnActorUnitSpawn += RegisterActor;
    }

    private void RegisterInitialActors()
    {
        foreach(ActorUnit actor in ActorUnitManager.Instance.ActorUnits)
        {
            RegisterActor(actor);
        }
    }

    private void RegisterActor(ActorUnit actor)
    {
        actor.GetComponent<ActorUnitHealthComponent>().OnTakeDamage += EnableOverlay;
    }

    private void DeregisterActor(ActorUnit actor)
    {
        actor.GetComponent<ActorUnitHealthComponent>().OnTakeDamage -= EnableOverlay;
    }
}
