using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Lane : MonoBehaviour, ICardTarget
{
    public const int kMaxMinionsInLane = 3;
    
    public Transform Anchor => this.transform;
    public List<Minion> Minions { get; } = new ();
    
    [field: SerializeField] public bool IsFriendly { get; private set; }

    bool ICardTarget.IsFriendly() => this.IsFriendly;
    Lane ICardTarget.Lane => this;

    public async UniTask ExecuteMinionActions()
    {
        foreach (Minion minion in Minions)
        {
            await minion.ExecuteAssignedAction();
        }
    }

    public async UniTask ClearBlock()
    {
        await UniTask.Delay(200);
        Minions.ForEach(minion => minion.ClearBlock());
    }
}