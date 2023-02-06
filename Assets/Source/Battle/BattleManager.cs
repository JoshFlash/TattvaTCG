using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private BattleGameService battleGameService;
    
    [SerializeField] private PlayerController playerOne = default;
    
    private void Start()
    {
        battleGameService = GameServices.Get<BattleGameService>();
        battleGameService.BeginBattle(playerOne).Forget();
    }
}