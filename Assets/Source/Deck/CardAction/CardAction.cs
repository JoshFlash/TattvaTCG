using System;
using UnityEngine;

public interface ICardAction
{
    void Invoke(ICharacter target);
    bool CanTarget(ICharacter target);
}

public abstract class CardAction<TModifier> : MonoBehaviour, ICardAction
{
    public enum TargetType { Enemy, Friendly, Any, None }
    
    [SerializeField] protected TModifier modifier;
    [SerializeField] protected TargetType targeting  = TargetType.Enemy;

    protected abstract void InvokeOnTarget(in ICharacter target, in TModifier modifier);

    public void Invoke(ICharacter target)
    {
        InvokeOnTarget(target, modifier);
    }

    public bool CanTarget(ICharacter target)
    {
        switch (targeting)
        {
            case TargetType.Enemy:
                return target != null && !target.IsFriendly();
            case TargetType.Friendly:
                return target != null && target.IsFriendly();
            case TargetType.Any:
                return true;
            case TargetType.None:
                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
