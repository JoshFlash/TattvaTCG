using Cysharp.Threading.Tasks;

public class BattleGameService : IGameService
{
    private Phase currentPhase;
    
    private PlayerController initiativePlayer = default;
    private PlayerController reactivePlayer = default;

    public void BeginBattle(PlayerController playerOne, PlayerController playerTwo)
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

    private async UniTask EndRound()
    {
        await UniTask.Yield();
        SwapInitiative();
        StartRound();
    }

    private void ProgressPhase()
    {
        ++currentPhase;
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
        
        await UniTask.Yield();
    }
    
    private async UniTask HandlePlayerTurnInitiative()
    {
        bool active = initiativePlayer.ActivateTurn();
        while (active)
        {
            active = await initiativePlayer.HandleInputOnTurn();
        }
    }
    
    private async UniTask HandlePlayerTurnReactive()
    {
        bool active = reactivePlayer.ActivateTurn();
        while (active)
        {
            active = await reactivePlayer.HandleInputOnTurn();
        }
    }

    private async UniTask HandleEndOfPhase()
    {
        await UniTask.Yield();
    }

    public void Init()
    {
        currentPhase = BattlePhase.Recovery;
    }
    public bool IsInitialized { get; set; }
}