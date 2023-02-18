using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TweenKey;
using UnityEngine;

public class BattleService : IGameService
{
    private Phase currentPhase;
    private int round = 0;
    private BattleDebugData battleDebugData = default;
    
    public PlayField PlayField { get; private set; } = default;

    public async UniTask BeginBattle(
        IPlayerController player, 
        IPlayerController opponent, 
        PlayField playField,
        UnityEngine.UI.Text manaText
    )
    {
        battleDebugData = GameServices.Get<DebugService>().BattleDebugData;
        PlayField = playField;

        var playerChamp = await SummonChampion(battleDebugData.PlayerChampionPrefab, PlayField.PlayerAnchor, player);
        playerChamp.OnManaChanged.AddListener((mana) => manaText.text = "Mana: " + mana.ToString());
        
        await SummonChampion(battleDebugData.EnemyChampionPrefab, PlayField.OpponentAnchor, opponent);

        await player.OnBattleStart();
        await opponent.OnBattleStart();

        await StartRound(player, opponent);
    }

    private async UniTask<Champion> SummonChampion(GameObject championPrefab, Transform championContainer, IPlayerController owner)
    {
        var champObject = GameObject.Instantiate(championPrefab, championContainer, false);
        var champion = champObject.GetComponent<Champion>();
        
        var angle = (championContainer.transform.position.x > 0 ? -1 : 1) * battleDebugData.ChampionCardRotation;
        champion.transform.TweenByRotation(Quaternion.AngleAxis(angle, -champion.transform.up), 0.5f);
        owner.AssignChampion(champion);

        await UniTask.Delay(1000);
        return champion;
    }

    private async UniTask StartRound(IPlayerController player, IPlayerController opponent)
    {
        ++round;
        Log.Info($"starting round: {round:00}", "[BATTLE]");

        await opponent.OnRoundStart();
        await player.OnRoundStart();

        ProgressPhase(player, opponent);
    }

    private void ProgressPhase(IPlayerController player, IPlayerController opponent)
    {
        currentPhase = currentPhase.Next();
        Log.Info($"phase progressed, phase is {currentPhase}", "[BATTLE]");

        CommencePhase(player, opponent).Forget();
    }

    private async UniTask CommencePhase(IPlayerController player, IPlayerController opponent)
    {
        bool active = await opponent.ActivateTurn(currentPhase);
        while (active) active = await opponent.HandleTurn(currentPhase);
                
        active = await player.ActivateTurn(currentPhase);
        while (active) active = await player.HandleTurn(currentPhase);
        
        await HandleEndOfPhase(player.Champion, opponent.Champion);
        if (!currentPhase.Equals(Phase.Recovery))
        {
            ProgressPhase(player, opponent);
        }
        else
        {
            await EndRound(player, opponent);
        }
    }

    private async UniTask HandleEndOfPhase(Champion playerChamp, Champion opponentChamp)
    {
        Log.Info("awaiting end of phase", "[BATTLE] ");

        if (currentPhase.Equals(Phase.Ability))
        {
            await playerChamp.ExecuteAllActions();
            await opponentChamp.ExecuteAllActions();
        }
        
        await UniTask.Yield();
    }

    private async UniTask EndRound(IPlayerController player, IPlayerController opponent)
    {
        Log.Info("awaiting end of round", "[BATTLE] ");
        await UniTask.Yield();

        await opponent.OnRoundEnd();
        await player.OnRoundEnd();
        await UniTask.Delay(1000);

        await StartRound(player, opponent);
    }

    public void Init()
    {
        currentPhase = Phase.Recovery;
    }

    public bool IsInitialized { get; set; }
}