using System;
using UnityEngine;

public class Minion : Character
{
    [SerializeField] private HealthBar healthBar = default;
    [SerializeField] private Transform modelParent = default;

    private void Start()
    {
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetCurrentHealth(health);
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