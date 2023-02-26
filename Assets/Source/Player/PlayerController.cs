using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public Champion Champion => champion;
    private Champion champion = default;
    
    [SerializeField] private PlayField playField = default;
    [SerializeField] private Button endTurnButton = default;
    [SerializeField] private Transform handAnchor = default;
    [SerializeField] private Transform drawAnchor = default;
    
    private InputHandler inputHandler = default;
    private BattleDeck battleDeck = default;

    private bool isTurnActive = false;

    private void Awake()
    {
        var debugDeck = new SavedDeck(GameServices.Get<DebugService>().DebugStarterDeck);
        battleDeck = new BattleDeck(debugDeck);
        inputHandler = new InputHandler(battleDeck.PlayerHand);
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

        foreach (var unit in playField.GetAllFriendlyUnits())
        {
            var unitCard = unit.GetComponent<PlayerCard>();
            if (unitCard && unitCard.ShouldMove())
            {
                unitCard.MoveToRequestedPosition(CardMovementConfig.MoveSpeed);
            }
        }
    }

    public void AssignChampion(Champion champion)
    {
        this.champion = champion;
        playField.PlayerChampion = champion;
    }

    public void OnChampionDefeated()
    {
        Log.Info($"Champion Defeated {Champion.name}");
        champion.OnManaChanged.RemoveAllListeners();
    }

    public async UniTask<bool> ActivateTurn(Phase phase)
    {
        endTurnButton.onClick.AddListener(EndTurn);

        await UniTask.Yield();

        inputHandler.SetPhase(phase);
        
        isTurnActive = true;
        return isTurnActive;
    }

    private void EndTurn()
    {
        if (isTurnActive)
        {
            endTurnButton.onClick.RemoveListener(EndTurn);
            isTurnActive = false;
        }
    }

    public async UniTask<bool> HandleTurn(Phase phase)
    {
        if (phase.Equals(Phase.Prep))
        {
            Log.NotImplemented("TODO - Implement Prep phase logic");
            // activate passives for player champ and minions
            isTurnActive = false;
            await UniTask.Delay(1000);
        }
        if (phase.Equals(Phase.Spell))
        {
            await HandleSpellPhaseInput();
        }
        else if (phase.Equals(Phase.Ability))
        {
            await HandleAbilityPhaseInput();
        }
        else
        {
            Log.NotImplemented($"TODO - Implement ${phase.Name} phase logic");
            isTurnActive = false;
        }

        await UniTask.Yield();
        return isTurnActive;
    }

    public async UniTask<bool> HandleEndOfPhase(Phase phase)
    {
        if (phase.Equals(Phase.Spell))
        {
            await battleDeck.DiscardHand(inputHandler);
        }

        if (phase.Equals(Phase.Ability))
        {
            await Champion.ExecuteAction();
            await playField.PlayerTopLane.ExecuteMinionActions();
            await playField.PlayerBottomLane.ExecuteMinionActions();
        }

        if (phase.Equals(Phase.Recovery))
        {
            Champion.ClearBlock();
            await playField.PlayerTopLane.ClearBlock();
            await playField.PlayerBottomLane.ClearBlock();
        }

        return true;
    }

    private async UniTask HandleAbilityPhaseInput()
    {
        if (inputHandler.IsReceivingInput(Phase.Ability))
        {
            inputHandler.UpdateUnitFocus();
            
            if (Input.GetMouseButtonDown(0))
            {
                await inputHandler.TryAssignActions();
            }
        }
    }

    private async UniTask HandleSpellPhaseInput()
    {
        if (inputHandler.IsReceivingInput(Phase.Spell))
        {
            inputHandler.UpdateCardFocus();
            if (Input.GetMouseButtonUp(0))
            {
                inputHandler.UpdateSelectedCard();
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (inputHandler.TryPlayCard(Champion.Mana, out PlayerCard card))
                {
                    bool success = await SelectTargetAndPlay(card);
                    if (!success)
                    {
                        inputHandler.ClearSelectedCard();
                    }
                }
            }
        }
    }

    private async UniTask<bool> SelectTargetAndPlay(PlayerCard card)
    {
        ICardTarget target = null;
        bool shouldCast = false;
        while (true)
        {
            await UniTask.Yield();

            if (Input.GetMouseButtonUp(1)) break;
            if (Input.GetMouseButtonUp(0))
            {
                shouldCast = true;
                var results = MainCamera.ScreenCast();
                foreach (var hit in results)
                {
                    if (hit.collider.TryGetComponent(out target)) break;
                }

                break;
            }
        }
        
        if (card.CanPlayOnTarget(target) && shouldCast)
        {
            int manaSpent = await battleDeck.PlayCardOnTarget(card, target, inputHandler, handAnchor);
            champion.SpendMana(manaSpent);

            return true;
        }

        return false;
    }

    public async UniTask OnBattleStart()
    {
        await battleDeck.ShuffleDeckIntoDrawPile();
    }

    public async UniTask OnRoundStart()
    {
        await battleDeck.DrawCards(GetCardDrawCount(), inputHandler, handAnchor, drawAnchor);
        inputHandler.UnlockAllCardsInHand();
    }

    public async UniTask OnRoundEnd()
    {
        await battleDeck.DiscardHand(inputHandler);
    }

    private int GetCardDrawCount()
    {
        champion.RestoreAllMana();
        return GameServices.Get<DebugService>().BattleDebugData.CardDrawPerTurn;
    }
}