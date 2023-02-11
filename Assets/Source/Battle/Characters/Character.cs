using System;
using UnityEngine;
using UnityEngine.Events;

public interface ICharacter
{
    void TakeDamage(int damage);
    void RestoreHealth(int healing);
    void SpendMana(int cost);
    void RestoreMana(int regen);
    void AssignCombatAction(CombatAction action);
    void AssignTarget(ICharacter character);
    bool IsFriendly();
}

public enum CombatAction { None, Attack, Defend, Cast }
public abstract class Character : MonoBehaviour, ICharacter
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

    public CombatAction SelectedAction { get; private set; } = CombatAction.None;
    public ICharacter Target { get; private set; } = default;

    private void LogStats()
    {
        Log.Info($"({name} stats) health: {Health}, mana: {Mana}, power: {Power}");
    }
    
    protected void Awake()
    {
        Health = MaxHealth;
        Mana = MaxMana;
        Power = BasePower;
        
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
        }
        
        LogStats();
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
        }
        
        LogStats();
    }

    public void AssignCombatAction(CombatAction action)
    {
        SelectedAction = action;
    }

    public void AssignTarget(ICharacter character)
    {
        Target = character;
    }

    bool ICharacter.IsFriendly()
    {
        return this.IsFriendly;
    }

    public void RestoreAllMana()
    {
        RestoreMana(MaxMana);
        
        LogStats();
    }
}