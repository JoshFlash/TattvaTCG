using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Champion : Character
{
    [field: SerializeField] public int MaxMana { get; private set; } = 1;
    public int Mana { get; private set; } = 1;
    
    public List<Minion> ControlledMinions = new ();

    [HideInInspector] public UnityEvent<int> OnManaChanged = new ();

    private void Awake()
    {
        Mana = MaxMana;
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
    
    public void SpendMana(int cost)
    {
        if (cost != 0)
        {
            Mana -= cost;
            OnManaChanged.Invoke(Mana);
        }
    }
    
    public void RestoreAllMana()
    {
        RestoreMana(MaxMana);
    }
}