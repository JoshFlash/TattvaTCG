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
}

public enum CombatAction { None, Attack, Defend, Cast }
public abstract class Character : MonoBehaviour, ICharacter
{
    [field: SerializeField] public int MaxHealth { get; private set; } = 10;
    [field: SerializeField] public int MaxMana { get; private set; } = 1;
    [field: SerializeField] public int BaseSpellPower { get; private set; } = 1;

    [HideInInspector] public UnityEvent<int> OnHealthChanged = new ();
    [HideInInspector] public UnityEvent<int> OnManaChanged = new ();
    
    public int Health { get; private set; } = 10;
    public int Mana { get; private set; } = 1;
    public int SpellPower { get; private set; } = 1;

    public CombatAction SelectedAction { get; private set; } = CombatAction.None;
    public ICharacter Target { get; private set; } = default;

    protected void Awake()
    {
        Health = MaxHealth;
        Mana = MaxMana;
        SpellPower = BaseSpellPower;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        OnHealthChanged.Invoke(Health);
    }

    public void RestoreHealth(int healing)
    {
        Health += healing;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        OnHealthChanged.Invoke(Health);
    }

    public void SpendMana(int cost)
    {
        if (cost != 0)
        {
            Mana -= cost;
            OnManaChanged.Invoke(Mana);
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
        }
    }

    public void AssignCombatAction(CombatAction action)
    {
        SelectedAction = action;
    }

    public void AssignTarget(ICharacter character)
    {
        Target = character;
    }

    public void RestoreAllMana()
    {
        RestoreMana(MaxMana);
    }
}