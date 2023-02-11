using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private Button endTurnButton = default;
    [SerializeField] private Transform handAnchor = default;
    
    private HandInputHandler handInputHandler = default; 
    private BattleDeck battleDeck = default;
    
    private Champion champion = default;
    private bool isTurnActive = false;

    private void Awake()
    {
        var debugDeck = new SavedDeck(GameServices.Get<DebugService>().DebugStarterDeck);
        battleDeck = new BattleDeck(debugDeck);
        handInputHandler = new HandInputHandler(battleDeck.PlayerHand);
    }

    private void Update()
    {
        
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            Time.timeScale *= 1.5f;
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            Time.timeScale /= 1.5f;
        #endif
        
    }

    private void LateUpdate()
    {
        foreach (var card in battleDeck.PlayerHand.GetMovingCards())
        {
            card.MoveToRequestedPosition(CardMovementConfig.MoveSpeed);
        }
    }

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
                    
        await battleDeck.DrawCards(GetCardDrawCount(), handInputHandler, handAnchor);
        handInputHandler.UnlockAllCards();
        
        return isTurnActive;
    }

    private int GetCardDrawCount()
    {
        return GameServices.Get<DebugService>().BattleDebugData.CardDrawPerTurn;
    }

    private void EndTurn()
    {
        if (isTurnActive)
        {
            endTurnButton.onClick.RemoveListener(EndTurn);
            battleDeck.DiscardHand(handInputHandler);
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
        if (handInputHandler.IsReceivingInput)
        {
            handInputHandler.UpdateCardFocus();
            if (Input.GetMouseButtonUp(0))
            {
                handInputHandler.UpdateSelectedCard();
                await UniTask.Yield();
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (handInputHandler.TryPlayCard(champion.Mana, out PlayerCard card))
                {
                    var target = await SelectTarget();
                    if (card.CanPlayOnTarget(target))
                    {
                        int manaSpent = await battleDeck.PlayCardOnTarget(card, target, handInputHandler, handAnchor);
                        champion.SpendMana(manaSpent);
                    }
                    else
                    {
                        handInputHandler.ClearSelectedCard();
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

    public async UniTask OnBattleStart()
    {
        await battleDeck.ShuffleDeckIntoDrawPile();
    }
}