using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class BattleGameService : IGameService
{
    private Phase currentPhase;
    
    private PlayerController initiativePlayer = default;
    private PlayerController reactivePlayer = default;
    
    public async UniTask BeginBattle(PlayerController playerOne, PlayerController playerTwo)
    {
        if (RandomChance.CoinFlip())
        {
            initiativePlayer = playerOne;
            reactivePlayer = playerTwo;
        }
        else
        {
            reactivePlayer = playerOne;
            initiativePlayer = playerTwo;
        }
        await initiativePlayer.SummonChampion("Alice");
        await reactivePlayer.SummonChampion("Robert");
        StartRound();
    }
    
    private void SwapInitiative()
    {
        (initiativePlayer, reactivePlayer) = (reactivePlayer, initiativePlayer);
    }

    private void StartRound()
    {
        ProgressPhase();
    }

    private void ProgressPhase()
    {
        currentPhase = currentPhase.Next();
        CommencePhase().Forget();
    }

    private async UniTask CommencePhase()
    {
        Log.Info($"phase progressed, phase is {BattlePhase.BattlePhaseAliases[currentPhase]}");
        await HandleStartOfPhase();
        Log.Info($"phase start, initiative is {initiativePlayer.Champion.name}");
        await HandlePlayerTurnInitiative();
        Log.Info($"turn ended, turn switching to {reactivePlayer.Champion.name}");
        await HandlePlayerTurnReactive();
        Log.Info($"turn ended, awaiting end of phase");
        await HandleEndOfPhase();
        if (!currentPhase.Equals(BattlePhase.Recovery))
        {
            ProgressPhase();
        }
        else
        {
            Log.Info("awaiting end of round");
            await EndRound();
        }
    }

    private async UniTask HandleStartOfPhase()
    {
        initiativePlayer.RestoreAllMana();
        reactivePlayer.RestoreAllMana();
        
        await UniTask.Yield();
    }
    
    private async UniTask HandlePlayerTurnInitiative()
    {
        await HandlePlayerTurn(initiativePlayer);
    }
    
    private async UniTask HandlePlayerTurnReactive()
    {
        await HandlePlayerTurn(reactivePlayer);
    }

    private async UniTask HandlePlayerTurn(PlayerController player)
    {
        bool active = player.ActivateTurn();
        while (active)
        {
            active = await player.HandleInputOnTurn();
        }
    }

    private async UniTask HandleEndOfPhase()
    {
        await UniTask.Yield();
    }
    
    private async UniTask EndRound()
    {
        await UniTask.Yield();
        SwapInitiative();
        StartRound();
    }

    public void Init()
    {
        currentPhase = BattlePhase.Recovery;
    }
    public bool IsInitialized { get; set; }
}