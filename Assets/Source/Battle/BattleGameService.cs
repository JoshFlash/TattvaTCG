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
        Log.Info("swapping initiative");
        (initiativePlayer, reactivePlayer) = (reactivePlayer, initiativePlayer);
    }

    private void StartRound()
    {
        Log.Info("starting new round");
        ProgressPhase();
    }

    private void ProgressPhase()
    {
        currentPhase = currentPhase.Next();
        Log.Info($"phase progressed, phase is {currentPhase}");

        CommencePhase().Forget();
    }

    private async UniTask CommencePhase()
    {
        await HandleStartOfPhase();
        await HandlePlayerTurnInitiative();
        await HandlePlayerTurnReactive();
        await HandleEndOfPhase();
        if (!currentPhase.Equals(BattlePhase.Recovery))
        {
            ProgressPhase();
        }
        else
        {
            await EndRound();
        }
    }

    private async UniTask HandleStartOfPhase()
    {
        initiativePlayer.RestoreAllMana();
        reactivePlayer.RestoreAllMana();
        Log.Info($"phase start, initiative belongs to {initiativePlayer.Champion.name}");

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
        Log.Info($"{player.Champion.name} turn started, awaiting input");
        
        bool active = await player.ActivateTurn();
        while (active)
        {
            active = await player.HandleInputOnTurn();
        }
    }

    private async UniTask HandleEndOfPhase()
    {
        Log.Info("awaiting end of phase");
        await UniTask.Yield();
    }
    
    private async UniTask EndRound()
    {
        Log.Info("awaiting end of round");
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