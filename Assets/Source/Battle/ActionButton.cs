using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    [field: SerializeField] public CombatAction CombatAction { get; private set; }

    public Character character { get; private set; } = default;

    private void Awake()
    {
        character = GetComponentInParent<Character>();
    }
}
