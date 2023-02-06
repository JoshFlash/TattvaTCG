using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class BattleGameService : IGameService
{
    private Phase currentPhase;
    
    private PlayerController initiativePlayer = default;
    
    public async UniTask BeginBattle(PlayerController playerOne)
    {
        initiativePlayer = playerOne;

        await initiativePlayer.SummonChampion("Alice");
        StartRound();
    }
    
    private void SwapInitiative()
    {
        Log.Info("swapping initiative");
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
        Log.Info($"phase start, initiative belongs to {initiativePlayer.Champion.name}");

        await UniTask.Yield();
    }
    
    private async UniTask HandlePlayerTurnInitiative()
    {
        await HandlePlayerTurn(initiativePlayer);
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