using Cysharp.Threading.Tasks;

public class BattleGameService : IGameService
{
    private Phase currentPhase;
    
    private PlayerController initiativePlayer = new ();
    private PlayerController reactivePlayer = new ();
    
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

    private void ChangeTurn()
    {
        initiativePlayer.IsTurnActive = !initiativePlayer.IsTurnActive;
        reactivePlayer.IsTurnActive = !initiativePlayer.IsTurnActive;
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
        ChangeTurn();
        await UniTask.Yield();
    }
    
    private async UniTask HandlePlayerTurnInitiative()
    {
        while (initiativePlayer.IsTurnActive)
        {
            await UniTask.Yield();
        }
    }
    
    private async UniTask HandlePlayerTurnReactive()
    {
        await UniTask.Yield();
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