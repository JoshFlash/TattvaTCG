using System;
using UnityEngine;

public class Minion : Character
{
    [SerializeField] private HealthBar healthBar = default;

    private void Start()
    {
        healthBar.SetMaxHealth(MaxHealth);
        healthBar.SetCurrentHealth(Health);
    }

    private void OnEnable()
    {
        OnHealthChanged.AddListener(healthBar.SetCurrentHealth);
    }
    
    private void OnDisable()
    {
        OnHealthChanged.RemoveListener(healthBar.SetCurrentHealth);
    }
}