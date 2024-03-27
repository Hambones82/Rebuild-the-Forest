using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionSource : MonoBehaviour
{
    private void OnDisable()
    {
        PollutionManager.Instance.NotifyOfSourceDeletion(this);
    }
}
