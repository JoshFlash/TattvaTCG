using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private BattleGameService battleGameService;
    
    [SerializeField] private PlayerController controllerOne = default;
    [SerializeField] private PlayerController controllerTwo = default;
    
    private void Start()
    {
        battleGameService = GameServices.GetService<BattleGameService>();
        battleGameService.BeginBattle((controllerOne.Player, controllerTwo.Player));
    }
}