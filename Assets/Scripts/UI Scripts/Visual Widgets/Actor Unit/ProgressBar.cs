using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private List<UnitActionType> doNotShowTypes;

    [SerializeField]
    private Transform progressBarTransform;

    [SerializeField]
    private Transform scaledBarTransform;


    [SerializeField]
    private UnitActionController actionController;

    private void Start()
    {
        actionController.OnActionStart += EnableBar;
        actionController.OnActionEnd += DisableBar;
        actionController.OnAdvanceAction += SetAmount;
    }

    private void OnEnable()
    {
        DisableBar();
    }
    
    private void EnableBar(UnitAction action)
    {
        if(!doNotShowTypes.Contains(action.actionType))
        {
            progressBarTransform.gameObject.SetActive(true);
        }
    }

    private void DisableBar()
    {
        progressBarTransform.gameObject.SetActive(false);
    }

    private void SetAmount(float amount)
    {
        scaledBarTransform.localScale = new Vector3(Mathf.Clamp(amount, 0, 1), transform.localScale.y, transform.localScale.z);
    }
}
