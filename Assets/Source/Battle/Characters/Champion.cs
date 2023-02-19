using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Champion : Character
{
    [field: SerializeField] public int MaxMana { get; private set; } = 1;
    public int Mana { get; private set; } = 1;

    [HideInInspector] public UnityEvent<int> OnManaChanged = new ();

    protected override void Start()
    {
        Mana = MaxMana;

        base.Start();
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

    public async UniTask ExecuteAction()
    {
        await ExecuteAssignedAction();

        await UniTask.Yield();
    }
}