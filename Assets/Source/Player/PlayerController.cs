using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private Button endTurnButton = default;
    [SerializeField] private BattleDeckController battleDeck = default;
    [SerializeField] private HandController handController = default;

    private Champion champion = default;
    private bool isTurnActive = false;

    public void AssignChampion(Champion champion)
    {
        this.champion = champion;
    }

    public void OnChampionDefeated()
    {
        Log.NotImplemented();
    }

    public async UniTask<bool> ActivateTurn()
    {
        endTurnButton.onClick.AddListener(EndTurn);

        // this will later be leveraged to skip turns where no actions are available to the player
        isTurnActive = true;
                    
        // debug code - deal hand
        int r = UnityEngine.Random.Range(5, 11);
        for (int i = 0; i < r; i++)
        {
            await battleDeck.AddCardToHand(handController);
        }
        handController.UnlockAllCards();
        
        return isTurnActive;
    }

    private void EndTurn()
    {
        if (isTurnActive)
        {
            endTurnButton.onClick.RemoveListener(EndTurn);
            handController.ClearAllCards();
            isTurnActive = false;
        }
    }

    public async UniTask<bool> HandleTurn()
    {
        await HandleInput();

        await UniTask.Yield();
        return isTurnActive;
    }

    private async UniTask HandleInput()
    {
        if (handController.IsReceivingInput)
        {
            handController.UpdateCardFocus();
            if (Input.GetMouseButtonUp(0))
            {
                handController.UpdateSelectedCard();
                await UniTask.Yield();
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (handController.TryPlaySelectedCard(champion.Mana))
                {
                    var target = await SelectTarget();
                    if (target is null)
                    {
                        handController.ClearSelectedCard();
                    }
                    else
                    {
                        int manaSpent = await handController.PlaySelectedCard(target);
                        champion.SpendMana(manaSpent);
                    }
                }

                await UniTask.Yield();
            }
        }
    }

    private async UniTask<ICharacter> SelectTarget()
    {
        ICharacter target = null;
        while (target is null)
        {
            await UniTask.Yield();

            if (Input.GetMouseButtonUp(1)) break;

            if (Input.GetMouseButtonUp(0))
            {
                var results = MainCamera.ScreenCast();
                foreach (var hit in results)
                {
                    if (hit.collider.TryGetComponent(out target)) break;
                }
            }
        }
        
        return target;
    }

    public void RestoreAllMana()
    {
        champion.RestoreAllMana();
    }
}