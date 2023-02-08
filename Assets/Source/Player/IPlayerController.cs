using Cysharp.Threading.Tasks;

public interface IPlayerController
{
    void AssignChampion(Champion champion);
    void OnChampionDefeated();
    UniTask<bool> ActivateTurn();
    UniTask<bool> HandleTurn();
    void RestoreAllMana();
}