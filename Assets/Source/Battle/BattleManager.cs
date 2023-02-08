using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private BattleGameService battleGameService;
    
    [SerializeField] private PlayerController player = default;
    [SerializeField] private OpponentController opponent = default;
    [SerializeField] private Transform playerChampParent = default;
    [SerializeField] private Transform opponentChampParent = default;
    
    private void Start()
    {
        battleGameService = GameServices.Get<BattleGameService>();
        battleGameService.BeginBattle(player, opponent, playerChampParent, opponentChampParent).Forget();
    }
}