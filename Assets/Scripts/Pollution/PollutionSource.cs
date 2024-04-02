using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionSource : MonoBehaviour
{
    [SerializeField]
    private List<PollutionEffect> sourceEffects;
    public IReadOnlyList<PollutionEffect> Effects { get { return sourceEffects; } }
    public void AddEffect(PollutionEffect effect)
    {
        if(!sourceEffects.Contains(effect)) sourceEffects.Add(effect);
    }
    private void OnDisable()
    {
        PollutionManager.Instance.NotifyOfSourceDeletion(this);
    }
}
