using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Button endTurnButton = default;
    [SerializeField] private BattleDeckController battleDeck = default;
    [SerializeField] private HandController handController = default;

    public Champion Champion { get; private set; }
    public Camera Camera { get; private set; }
    
    private bool isTurnActive = false;

    private void Awake()
    {
        Camera ??= Camera.main;
    }

    public async UniTask SummonChampion(string championName)
    {
        Champion = new GameObject(championName).AddComponent<Champion>();
        await Task.Delay(1000);
    }

    public void OnChampionDefeated()
    {
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

    public async UniTask<bool> HandleInputOnTurn()
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
                int manaSpent = await handController.TryPlaySelectedCard(Champion.Mana, Champion.SpellPower);
                Champion.SpendMana(manaSpent);
            }
        }

        var right = Input.GetMouseButtonUp(1);
        var left = Input.GetMouseButtonUp(0);
        if (right || left)
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                if (Champion.Mana > 0 && hit.transform.TryGetComponent<Character>(out var character))
                {
                    Champion.SpendMana(1);
                    if (left)
                    {
                        BattleActions.DamageCharacter(character, 5);
                    }
                    else
                    {
                        BattleActions.HealCharacter(character, 5);
                    }
                }
            }
        }

        await UniTask.Yield();
        return isTurnActive;
    }

    public void RestoreAllMana()
    {
        Champion.RestoreAllMana();
    }
}