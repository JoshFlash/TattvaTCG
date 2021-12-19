using UnityEngine;public interface ICharacter
{
    void TakeDamage(float damage);
}

public abstract class Character : MonoBehaviour, ICharacter
{
    [SerializeField] private CharacterStats stats = default;
    
    public System.Action<float> OnHealthReduced = delegate {};
    
    private float health = 40f;
    public void TakeDamage(float damage)
    {
        health -= damage;
        OnHealthReduced.Invoke(health);
    }
}