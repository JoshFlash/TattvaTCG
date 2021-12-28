using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Champion Champion { get; set; }
    public Camera Camera { get; set; }
    
    public bool IsTurnActive { get; set; }

    private void Awake()
    {
        Camera ??= Camera.main;
    }

    private void Start()
    {
        Champion.OnManaChanged.AddListener(HandleManaChanged);
    }

    private void HandleManaChanged(int manaRemaining)
    {
        Log.Info($"mana changed {manaRemaining}");
        if (manaRemaining <= 0)
        {
            IsTurnActive = false;
        }
    }

    private void Update()
    {
        if (!IsTurnActive)
            return;

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
    }
}