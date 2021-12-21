using System;
using UnityEngine;
using UnityEngine.Events;

public interface ICharacter
{
    void TakeDamage(float damage);
}

public abstract class Character : MonoBehaviour, ICharacter
{
    [SerializeField] protected CharacterStats stats = default;
    
    protected UnityEvent<float> OnHealthChanged = new ();
    
    protected float health = 10f;

    protected void Awake()
    {
        health = stats.MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        OnHealthChanged.Invoke(health);
    }

    public void Heal(float healing)
    {
        health += healing;
        if (health > stats.MaxHealth)
        {
            health = stats.MaxHealth;
        }
        OnHealthChanged.Invoke(health);
    }
}