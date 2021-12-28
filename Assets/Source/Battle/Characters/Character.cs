using System;
using UnityEngine;
using UnityEngine.Events;

public interface ICharacter
{
    void TakeDamage(int damage);
    void RestoreHealth(int healing);
    void SpendMana(int cost);
    void RestoreMana(int regen);
}

public abstract class Character : MonoBehaviour, ICharacter
{
    [SerializeField] protected int maxHealth = default;
    [SerializeField] protected int maxMana = default;
    [SerializeField] protected int baseSpellPower = default;
    
    [HideInInspector] public UnityEvent<int> OnHealthChanged = new ();
    [HideInInspector] public UnityEvent<int> OnManaChanged = new ();
    
    protected int health = 1;
    protected int mana = 1;
    protected int spellPower = 1;

    protected void Awake()
    {
        health = maxHealth;
        mana = maxMana;
        spellPower = baseSpellPower;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnHealthChanged.Invoke(health);
    }

    public void RestoreHealth(int healing)
    {
        health += healing;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        OnHealthChanged.Invoke(health);
    }

    public void SpendMana(int cost)
    {
        mana -= cost;
        OnManaChanged.Invoke(mana);
    }

    public void RestoreMana(int regen)
    {
        mana += regen;
        if (mana > maxMana)
        {
            mana = maxMana;
        }
        OnManaChanged.Invoke(mana);
    }
}