using UnityEngine;

public interface ICardAction
{
    void Invoke(ICharacter target);
}

public abstract class CardAction<TModifier> : MonoBehaviour, ICardAction
{
    [SerializeField] protected TModifier modifier;

    protected abstract void InvokeOnTarget(in ICharacter target, in TModifier modifier);

    public void Invoke(ICharacter target)
    {
        InvokeOnTarget(target, modifier);
    }
}
