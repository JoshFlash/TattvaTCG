using Cysharp.Threading.Tasks;
using TweenKey;
using TweenKey.Interpolation;
using UnityEngine;


public class Card : MonoBehaviour
{
    private Tween<Vector3> moveTween = null;
    
    public Vector3 defaultPosition = default;
    private Vector3 targetPositionCached  = default;
    private Vector3 targetPositionRequested  = default;

    public CardState State { get; private set; } = CardState.Default;
    public bool LockPosition { get; set;} = true;

    public bool ShouldMove()
    {
        return !LockPosition && targetPositionCached != targetPositionRequested;
    }
    
    public void CachePosition()
    {
        defaultPosition = transform.position;
    }

    private void Awake()
    {
        LockPosition = true;
    }

    public void SetState(CardState cardState, Vector3 requestedOffset)
    {
        State = cardState;
        targetPositionRequested = defaultPosition + transform.rotation * requestedOffset;
    }

    public void TweenToPosition(Vector3 position, float duration, System.Action onComplete = null)
    {
        targetPositionRequested = targetPositionCached = position;
        moveTween?.Cancel(true);
        moveTween = transform.TweenMove(targetPositionRequested, duration, onComplete, Easing.Cubic.Out, 0f);
    }

    public void MoveToRequestedPosition(float duration, System.Action onComplete = null)
    {
        if (State.Equals(CardState.Examine))
        {
            SetPosition(targetPositionRequested);
            return;
        }
        
        if (State.Equals(CardState.Clear))
        {
            SetPosition(targetPositionRequested);
            targetPositionRequested = defaultPosition;
            LockPosition = true;
            onComplete += () => LockPosition = false;
        }
        
        TweenToPosition(targetPositionRequested, duration, onComplete);
    }

    private void SetPosition(Vector3 position)
    {
        if (!LockPosition)
        {
            targetPositionCached = position;
            transform.position = position;
            moveTween?.Cancel(true);
        }
    }
}