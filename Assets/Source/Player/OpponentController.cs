
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OpponentController : MonoBehaviour, IPlayerController
{
    public async UniTask SummonChampion(string resourcePath)
    {
        Log.NotImplemented();
        await UniTask.Yield();
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
}