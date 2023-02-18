using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum CombatAction { None, Attack, Defend, Cast }
public abstract class Character : MonoBehaviour, ITarget
{
    [field: SerializeField] public TMP_Text PowerText { get; private set; } 
    [field: SerializeField] public TMP_Text HealthText { get; private set; } 
    
    [field: SerializeField] public int MaxHealth { get; private set; } = 10;
    [field: SerializeField] public int BasePower { get; private set; } = 1;
    [field: SerializeField] public bool IsFriendly { get; private set; } = false;

    [HideInInspector] public UnityEvent<int> OnHealthChanged = new ();
    [HideInInspector] public UnityEvent<int> OnPowerChanged = new ();
    
    public int Health { get; private set; } = 10;
    public int Power { get; private set; } = 1;

    public Lane Lane { get; set; } = default;
    public CombatAction SelectedAction { get; set; } = CombatAction.None;
    public ITarget Target { get; set; } = default;

    private void LogStats()
    {
        Log.Info($"({name} stats) health: {Health}, power: {Power}");
    }
    
    protected void Start()
    {
        Health = MaxHealth;
        Power = BasePower;
        
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        PowerText.text = Power.ToString();
        HealthText.text = Health.ToString();
        
        LogStats();
    }

    public void SetStatsOnSummon(int power, int health)
    {
        Power = power;
        Health = health;

        UpdateStatusText();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnHealthChanged.Invoke(Health);
        
        UpdateStatusText();
    }

    public void RestoreHealth(int healing)
    {
        Health += healing;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        OnHealthChanged.Invoke(Health);
        
        UpdateStatusText();
    }

    bool ITarget.IsFriendly()
    {
        return this.IsFriendly;
    }

    public void DisplayActions()
    {
        throw new NotImplementedException();
    }
}