using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private BattleGameService battleGameService;
    
    [SerializeField] private PlayerController playerOne = default;
    [SerializeField] private PlayerController playerTwo = default;
    
    private void Start()
    {
        battleGameService = GameServices.GetService<BattleGameService>();
        battleGameService.BeginBattle(playerOne, playerTwo).Forget();
    }
}