using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private BattleService battleService;
    
    [SerializeField] private PlayerController player = default;
    [SerializeField] private OpponentController opponent = default;

    [SerializeField] private PlayField playField = default;
    
    private void Start()
    {
        battleService = GameServices.Get<BattleService>();
        battleService.BeginBattle(player, opponent, playField).Forget();
    }
}