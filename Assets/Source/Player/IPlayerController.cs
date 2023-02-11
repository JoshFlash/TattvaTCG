using Cysharp.Threading.Tasks;

public interface IPlayerController
{
    void AssignChampion(Champion champion);
    void OnChampionDefeated();
    
    UniTask<bool> ActivateTurn(Phase phase);
    UniTask<bool> HandleTurn(Phase phase);
    
    UniTask OnBattleStart();
    UniTask OnRoundStart();
    UniTask OnRoundEnd();
}