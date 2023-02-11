using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OpponentController : MonoBehaviour, IPlayerController
{
    private List<Champion> champions = new();

    public void AssignChampion(Champion champion)
    {
        champions.Add(champion);
    }

    public void OnChampionDefeated()
    {
        Log.NotImplemented();
    }

    public async UniTask<bool> ActivateTurn()
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

    public void RestoreAllMana()
    {
        Log.NotImplemented();
    }

    public async UniTask OnBattleStart()
    {
        Log.NotImplemented();
    }
}