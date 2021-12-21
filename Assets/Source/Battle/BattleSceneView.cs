using System;
using UnityEngine;

public class BattleSceneView : MonoBehaviour
{
    private BattleManager battleManager = new ();
    
    [SerializeField] private PlayerController controllerOne = default;
    [SerializeField] private PlayerController controllerTwo = default;

    private void Start()
    {
        battleManager.BeginBattle((controllerOne.Player, controllerTwo.Player));
    }
}