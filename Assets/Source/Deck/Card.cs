using Cysharp.Threading.Tasks;
using TweenKey;
using TweenKey.Interpolation;
using UnityEngine;


public class Card : MonoBehaviour
{
    private Tween<Vector3> moveTween = null;
    private Vector3 defaultPosition = default;

    public Vector3 TargetPositionCached { get; private set; }
    public Vector3 TargetPositionRequested { get; private set; }

    public CardState State { get; set; } = CardState.Default;
    public int Index { get; init; }= -1;
    public bool LockPosition { get; set;} = true;
    
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
        TargetPositionRequested = defaultPosition + transform.rotation * requestedOffset;
    }

    public void TweenToPosition(Vector3 position, float duration, System.Action onComplete = null)
    {
        TargetPositionRequested = TargetPositionCached = position;
        moveTween?.Cancel(true);
        moveTween = transform.TweenMove(TargetPositionRequested, duration, onComplete, Easing.Cubic.Out, 0f);
    }

    public void MoveToRequestedPosition(float duration, System.Action onComplete = null)
    {
        if (State.Equals(CardState.Examine))
        {
            SetPosition(TargetPositionRequested);
            return;
        }
        
        if (State.Equals(CardState.Clear))
        {
            SetPosition(TargetPositionRequested);
            TargetPositionRequested = defaultPosition;
        }
        
        TweenToPosition(TargetPositionRequested, duration, onComplete);
    }

    private void SetPosition(Vector3 position)
    {
        if (!LockPosition)
        {
            TargetPositionCached = position;
            transform.position = position;
            moveTween?.Cancel(true);
        }
    }
}