using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Champion Champion { get; set; }
    public Camera Camera { get; set; }
    
    private bool isTurnActive = false;

    private void Awake()
    {
        Camera ??= Camera.main;
        Champion = new GameObject("Champion").AddComponent<Champion>();
    }

    private void Start()
    {
        Champion.OnManaChanged.AddListener(HandleManaChanged);
    }

    private void HandleManaChanged(int manaRemaining)
    {
        if (manaRemaining <= 0)
        {
            isTurnActive = false;
        }
    }

    public bool ActivateTurn()
    {
        // this will later be leveraged to skip turns where no actions are available to the player
        isTurnActive = true;
        return isTurnActive;
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
                if (hit.transform.TryGetComponent<Character>(out var character))
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