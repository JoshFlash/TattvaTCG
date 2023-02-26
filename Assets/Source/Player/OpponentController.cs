using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OpponentController : MonoBehaviour, IPlayerController
{
    private List<Champion> champions = new();
    public Champion Champion => champions[0];
    
    [SerializeField] private PlayField playField = default;
    public PlayField PlayField => playField;

    public void AssignChampion(Champion champion)
    {
        champions.Add(champion);
        playField.OpponentChampion = champion;
    }

    public void OnChampionDefeated()
    {
        Log.NotImplemented();
    }

    public async UniTask<bool> HandleTurn(Phase phase)
    {
        Log.NotImplemented();
        await UniTask.Yield();
        return false;
    }

    public async UniTask<bool> HandleEndOfPhase(Phase phase)
    {
        if (phase.Equals(Phase.Ability))
        {
            await Champion.ExecuteAction();
            await playField.OpponentTopLane.ExecuteMinionActions();
            await playField.OpponentBottomLane.ExecuteMinionActions();
        }

        if (phase.Equals(Phase.Recovery))
        {
            Champion.ClearBlock();
            await playField.OpponentTopLane.ClearBlock();
            await playField.OpponentBottomLane.ClearBlock();
        }

        return true;
    }

    public async UniTask<bool> ActivateTurn(Phase phase)
    {
        Log.NotImplemented();
        await UniTask.Yield();
        return false;
    }

    public async UniTask OnBattleStart()
    {
        await UniTask.Yield();
        Log.NotImplemented();
    }

    public async UniTask OnRoundStart()
    {
        await UniTask.Yield();
        Log.NotImplemented();
    }
    
    public async UniTask OnRoundEnd()
    {
        await UniTask.Yield();
        Log.NotImplemented();
    }
}