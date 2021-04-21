using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    [SerializeField]
    private PlayerResourceData playerData;

    public TurnController turnController;

    public void EndTurn()
    {
        turnController.EndTurn();
        //playerData.TurnReset();
    }
}
