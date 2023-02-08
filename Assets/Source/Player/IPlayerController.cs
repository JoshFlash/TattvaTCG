using Cysharp.Threading.Tasks;

public interface IPlayerController
{
    UniTask SummonChampion(string resourcePath);
    void OnChampionDefeated();
    UniTask<bool> ActivateTurn();
    UniTask<bool> HandleTurn();
    void RestoreAllMana();
}