using Cysharp.Threading.Tasks;

public interface IPlayerController
{
    Champion Champion { get; }
    void AssignChampion(Champion champion);
    void OnChampionDefeated();
    
    UniTask<bool> ActivateTurn(Phase phase);
    UniTask<bool> HandleTurn(Phase phase);
    UniTask<bool> HandleEndOfPhase(Phase phase, PlayField playField);
    
    UniTask OnBattleStart();
    UniTask OnRoundStart();
    UniTask OnRoundEnd();
}