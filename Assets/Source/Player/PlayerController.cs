using System;
using System.Collections.Generic;
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
        await UniTask.Delay(1000);
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
                if (handController.TryPlaySelectedCard(Champion.Mana))
                {
                    var target = await SelectTarget();
                    if (target is null)
                    {
                        handController.ClearSelectedCard();
                    }
                    else
                    {
                        int manaSpent = await handController.PlaySelectedCard(target);
                        Champion.SpendMana(manaSpent);
                    }
                }
                await UniTask.Yield();
            }
        }

        await UniTask.Yield();
        return isTurnActive;
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
        Champion.RestoreAllMana();
    }
}