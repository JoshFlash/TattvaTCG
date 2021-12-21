using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player Player = new ();
    public Camera Camera { get; set; }

    [SerializeField] private List<Minion> controlledMinions = new ();
    [SerializeField] private List<Champion> controlledChampions = new ();

    private void Awake()
    {
        Camera ??= Camera.main;
    }

    private void Update()
    {
        if (!Player.IsTurnActive)
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
                    if (left)
                    {
                        var spell = new DamageSpell { Damage = 5 };
                        spell.Cast(character);
                    }
                    else
                    {
                        character.Heal(5);
                    }
                }
            }
        }
    }
}