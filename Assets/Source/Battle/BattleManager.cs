using System.Threading.Tasks;

public class BattleManager
{
    private Phase currentPhase = new (-1);
    
    private Player initiativePlayer = new ();
    private Player reactivePlayer = new ();
    
    public void BeginBattle((Player one, Player two) player)
    {
        if (RandomChance.CoinFlip())
        {
            initiativePlayer = player.one;
            reactivePlayer = player.two;
        }
        else
        {
            reactivePlayer = player.one;
            initiativePlayer = player.two;
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

    private async Task EndRound()
    {
        await Task.Yield();
        SwapInitiative();
        StartRound();
    }

    private void ProgressPhase()
    {
        ++currentPhase;
        CommencePhase();
    }

    private async void CommencePhase()
    {
        await HandleStartOfPhase();
        await HandlePlayerTurnInitiative();
        await HandlePlayerTurnReactive();
        await HandleEndOfPhase();
        if (!currentPhase.Equals(Phase.Recovery))
        {
            ProgressPhase();
        }
        else
        {
            await EndRound();
        }
    }
    
    private async Task HandleStartOfPhase()
    {
        ChangeTurn();
        await Task.Yield();
    }
    
    private async Task HandlePlayerTurnInitiative()
    {
        while (initiativePlayer.IsTurnActive)
        {
            await Task.Yield();
        }
        
    }
    
    private async Task HandlePlayerTurnReactive()
    {
        await Task.Yield();
    }

    private async Task HandleEndOfPhase()
    {
        await Task.Yield();
    }

}