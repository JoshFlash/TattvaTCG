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
    [SerializeField] private int maxHealth = 10;
    public int MaxHealth => maxHealth;
    [SerializeField] private int maxMana = 1;
    public int MaxMana => maxMana;
    [SerializeField] private int baseSpellPower = 1;
    public int BaseSpellPower => baseSpellPower;

    [SerializeField] private GameObject model = default;
    [SerializeField] private Transform modelParent = default;

    [HideInInspector] public UnityEvent<int> OnHealthChanged = new ();
    [HideInInspector] public UnityEvent<int> OnManaChanged = new ();
    
    private int health = 10;
    public int Health => health;
    private int mana = 1;
    public int Mana => mana;
    private int spellPower = 1;
    public int SpellPower => spellPower;

    protected void Awake()
    {
        if (model)
            Instantiate(model, modelParent);
        
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
        if (cost != 0)
        {
            mana -= cost;
            OnManaChanged.Invoke(mana);
        }
    }

    public void RestoreMana(int regen)
    {
        if (regen != 0)
        {
            mana += regen;
            if (mana > maxMana)
            {
                mana = maxMana;
            }

            OnManaChanged.Invoke(mana);
        }
    }

    public void RestoreAllMana()
    {
        RestoreMana(maxMana);
    }
}