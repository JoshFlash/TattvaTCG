using System;
using UnityEngine;

public interface ICardAction
{
    void Invoke(ICardTarget target);
    bool CanTarget(ICardTarget target);
}

public abstract class CardAction<TModifier> : MonoBehaviour, ICardAction
{
    [Flags]
    protected enum Faction
    {
        Friendly = 1,
        Enemy = 2,
    }
    
    [Flags]
    protected enum TargetType
    {
        Champion = 1,
        Minion = 2,
        Lane = 4,
    }
    
    [SerializeField] protected TModifier modifier;
    [SerializeField] protected Faction targetFaction  = Faction.Enemy;
    [SerializeField] protected TargetType targetType  = TargetType.Minion;

    protected abstract void InvokeOnTarget(in ICardTarget target, in TModifier modifier);

    public void Invoke(ICardTarget target)
    {
        InvokeOnTarget(target, modifier);
    }

    public bool CanTarget(ICardTarget target)
    {
        if (target is null) return false;
        
        return CanTargetFaction(target) && CanTargetType(target);
    }

    private bool CanTargetFaction(ICardTarget target)
    {
        if ((targetFaction & Faction.Friendly) != 0 && target.IsFriendly())
            return true;
        
        if ((targetFaction & Faction.Enemy) != 0 && !target.IsFriendly())
            return true;

        return false;
    }

    private bool CanTargetType(ICardTarget target)
    {
        if ((targetType & TargetType.Champion) != 0 && target is Champion)
            return true;

        if ((targetType & TargetType.Minion) != 0 && target is Minion)
            return true;

        if ((targetType & TargetType.Lane) != 0 && target.Lane is not null)
            return true;
        
        return false;
    }
}
