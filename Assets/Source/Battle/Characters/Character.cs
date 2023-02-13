using System;
using UnityEngine;
using UnityEngine.Events;

public interface ITarget
{
    bool IsFriendly();
    Lane GetLane();
}

public enum CombatAction { None, Attack, Defend, Cast }
public abstract class Character : MonoBehaviour, ITarget
{
    [field: SerializeField] public GameObject PowerText { get; private set; } 
    [field: SerializeField] public GameObject HealthText { get; private set; } 
    
    [field: SerializeField] public int MaxHealth { get; private set; } = 10;
    [field: SerializeField] public int MaxMana { get; private set; } = 1;
    [field: SerializeField] public int BasePower { get; private set; } = 1;
    [field: SerializeField] public bool IsFriendly { get; private set; } = false;

    [HideInInspector] public UnityEvent<int> OnHealthChanged = new ();
    [HideInInspector] public UnityEvent<int> OnManaChanged = new ();
    
    public int Health { get; private set; } = 10;
    public int Mana { get; private set; } = 1;
    public int Power { get; private set; } = 1;

    public Lane Lane { get; set; } = default;
    public CombatAction SelectedAction { get; set; } = CombatAction.None;
    public ITarget Target { get; set; } = default;

    private void LogStats()
    {
        Log.Info($"({name} stats) health: {Health}, mana: {Mana}, power: {Power}");
    }
    
    protected void Awake()
    {
        Health = MaxHealth;
        Mana = MaxMana;
        Power = BasePower;
    }

    public void SetStatsOnSummon(int power, int health, int mana)
    {
        Power = power;
        Health = health;
        Mana = mana;
        
        LogStats();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnHealthChanged.Invoke(Health);
        
        LogStats();
    }

    public void RestoreHealth(int healing)
    {
        Health += healing;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        OnHealthChanged.Invoke(Health);
        LogStats();
    }

    public void SpendMana(int cost)
    {
        if (cost != 0)
        {
            Mana -= cost;
            OnManaChanged.Invoke(Mana);
            LogStats();
        }
    }

    public void RestoreMana(int regen)
    {
        if (regen != 0)
        {
            Mana += regen;
            if (Mana > MaxMana)
            {
                Mana = MaxMana;
            }

            OnManaChanged.Invoke(Mana);
            LogStats();
        }
    }
    
    public void RestoreAllMana()
    {
        RestoreMana(MaxMana);
    }
    
    bool ITarget.IsFriendly()
    {
        return this.IsFriendly;
    }

    public Character GetCharacter()
    {
        return this;
    }

    public Lane GetLane()
    {
        return this.Lane;
    }
}