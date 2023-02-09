using Cysharp.Threading.Tasks;
using TweenKey;
using UnityEngine;

public class BattleGameService : IGameService
{
    private Phase currentPhase;
    private int round = 0;
    private BattleDebugData battleDebugData = default;

    public async UniTask BeginBattle(
        IPlayerController player, 
        IPlayerController opponent, 
        Transform playerChampParent, 
        Transform opponentChampParent
    )
    {
        battleDebugData = GameServices.Get<DebugService>().BattleDebugData;

        await SummonChampion(battleDebugData.PlayerChampionPrefab, playerChampParent, player);
        await SummonChampion(battleDebugData.EnemyChampionPrefab, opponentChampParent, opponent);

        StartRound(player, opponent);
    }

    private async UniTask SummonChampion(GameObject championPrefab, Transform championContainer, IPlayerController owner)
    {
        var champObject = GameObject.Instantiate(championPrefab, championContainer, false) as GameObject;
        var champion = champObject.GetComponent<Champion>();
        
        var angle = (championContainer.transform.position.x > 0 ? -1 : 1) * battleDebugData.ChampionCardRotation;
        champion.transform.TweenByRotation(Quaternion.AngleAxis(angle, -champion.transform.up), 0.5f);
        owner.AssignChampion(champion);

        await UniTask.Delay(1000);
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
        if (!currentPhase.Equals(Phase.Recovery))
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
        currentPhase = Phase.Recovery;
    }

    public bool IsInitialized { get; set; }
}