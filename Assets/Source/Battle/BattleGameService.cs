using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public class BattleGameService : IGameService
{
    private Phase currentPhase;
    private int round = 0;
    
    public async UniTask BeginBattle(IPlayerController playerOne, IPlayerController playerTwo)
    {
        const string _DEBUG_championResource = "Cards/Champions/Helf";
        await playerOne.SummonChampion(_DEBUG_championResource);
        await playerTwo.SummonChampion("Nulliam");
        StartRound(playerOne, playerTwo);
    }

    private void StartRound(IPlayerController player, IPlayerController opponent)
    {
        ++round;
        Log.Info($"starting round: {round:00}");
        
        ProgressPhase(player, opponent);
    }

    private void ProgressPhase(IPlayerController player, IPlayerController opponent)
    {
        currentPhase = currentPhase.Next();
        Log.Info($"phase progressed, phase is {currentPhase}");

        CommencePhase(player, opponent).Forget();
    }

    private async UniTask CommencePhase(IPlayerController player, IPlayerController opponent)
    {
        await HandleStartOfPhase(opponent);
        await HandleStartOfPhase(player);
        await HandlePlayerTurn(opponent);
        await HandlePlayerTurn(player);
        await HandleEndOfPhase();
        if (!currentPhase.Equals(BattlePhase.Recovery))
        {
            ProgressPhase(player, opponent);
        }
        else
        {
            await EndRound(player, opponent);
        }
    }

    private async UniTask HandleStartOfPhase(IPlayerController player)
    {
        Log.Info($"phase start: {currentPhase}");

        player.RestoreAllMana();

        await UniTask.Yield();
    }

    private async UniTask HandlePlayerTurn(IPlayerController player)
    {
        Log.Info($"{player} turn started, awaiting input");
        
        bool active = await player.ActivateTurn();
        while (active)
        {
            active = await player.HandleTurn();
        }
    }

    private async UniTask HandleEndOfPhase()
    {
        Log.Info("awaiting end of phase");
        await UniTask.Yield();
    }
    
    private async UniTask EndRound(IPlayerController player, IPlayerController opponent)
    {
        Log.Info("awaiting end of round");
        await UniTask.Yield();

        StartRound(player, opponent);
    }

    public void Init()
    {
        currentPhase = BattlePhase.Recovery;
    }
    public bool IsInitialized { get; set; }
}