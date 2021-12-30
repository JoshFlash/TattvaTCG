using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Button endTurnButton = default;
    [SerializeField] private BattleDeckController battleDeck = default;
    public Champion Champion { get; set; }
    public Camera Camera { get; set; }
    
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

    public bool ActivateTurn()
    {
        endTurnButton.onClick.AddListener(EndTurn);

        // this will later be leveraged to skip turns where no actions are available to the player
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
    
    private void Update()
    {
        Card card = battleDeck.CheckMouseoverCard();
        if (Input.GetMouseButtonUp(0))
        {
            if (battleDeck.TrySelectCard(card))
            {
                return;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            // play selected card
        }
            
        if (card != null)
        {
            battleDeck.UpdateExaminedCard(card);
        }
        else if (battleDeck.ShouldClearExaminedCard())
        {
            battleDeck.ClearExaminedCard();
        }
    }

    public async UniTask<bool> HandleInputOnTurn()
    {
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