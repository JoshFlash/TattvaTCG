using System;
using TweenKey;
using TweenKey.Interpolation;
using UnityEngine;

[RequireComponent(typeof(ICardAction))]
public class Card : MonoBehaviour
{
    [field: SerializeField] public int ManaCost { get; private set; } = 1;

    public Vector3 DefaultPosition { get; private set; }
    public  bool LockInteraction { get; private set; }
    
    private CardState state = CardState.Default;
    private Vector3 targetPositionCached = default;
    private Vector3 targetPositionRequested = default;
    private Tween<Vector3> moveTween = null;

    private ICardAction cardAction = default;

    public bool ShouldMove()
    {
        return !LockInteraction && targetPositionCached != targetPositionRequested;
    }
    
    public void CachePosition()
    {
        DefaultPosition = transform.position;
    }

    private void Awake()
    {
        Lock();
    }

    private void Start()
    {
        cardAction = GetComponent<ICardAction>();
    }

    public void Lock()
    {
        LockInteraction = true;
    }

    public void Unlock()
    {
        LockInteraction = false;
    }

    public void SetState(CardState cardState)
    {
        state = cardState;
        targetPositionRequested = DefaultPosition + transform.rotation * state.Offset;
    }

    public void TweenToPosition(Vector3 position, float duration, System.Action onComplete = null)
    {
        targetPositionRequested = targetPositionCached = position;
        moveTween?.Cancel(true);
        moveTween = transform.TweenMove(targetPositionRequested, duration, onComplete, Easing.Cubic.Out, 0f);
    }

    public void MoveToRequestedPosition(float duration)
    {
        Action onComplete = delegate {};

        if (state.Equals(CardState.Examine))
        {
            SetPosition(targetPositionRequested);
            return;
        }
        
        if (state.Equals(CardState.ClearFocus))
        {
            SetPosition(targetPositionRequested);
            targetPositionRequested = DefaultPosition;
            Lock();
            onComplete += Unlock;
        }
        
        TweenToPosition(targetPositionRequested, duration, onComplete);
    }

    private void SetPosition(Vector3 position)
    {
        if (!LockInteraction)
        {
            targetPositionCached = position;
            transform.position = position;
            moveTween?.Cancel(true);
        }
    }

    public bool PlayCard(ICharacter target)
    {
        if (!LockInteraction)
        {
            Lock();
            cardAction.Invoke(target);
            return true;
        }

        return false;
    }
}