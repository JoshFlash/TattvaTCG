using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OpponentController : MonoBehaviour, IPlayerController
{
    private List<Champion> champions = new();
    public Champion Champion => champions[0];

    public void AssignChampion(Champion champion)
    {
        champions.Add(champion);
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

    public async UniTask<bool> ActivateTurn(Phase phase)
    {
        Log.NotImplemented();
        await UniTask.Yield();
        return false;
    }

    public async UniTask<bool> HandleTurn()
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