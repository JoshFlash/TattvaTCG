using System.Collections;
using TweenKey;
using TweenKey.Interpolation;
using UnityEngine;

[RequireComponent(typeof(ICardAction))]
public class Card : MonoBehaviour
{
    [field: SerializeField] public int ManaCost { get; private set; } = 1;

    public Vector3 DefaultPosition { get; private set; }
    public bool BlockMouseover { get; private set; } = false;
    
    private CardState state = CardState.Default;
    private Vector3 targetPositionCached = default;
    private Vector3 targetPositionRequested = default;
    private Tween<Vector3> moveTween = null;
    private bool lockInteraction = false;

    private ICardAction cardAction = default;

    public bool ShouldMove()
    {
        return !lockInteraction && targetPositionCached != targetPositionRequested;
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

    // This is for edge cases where cards can fight for focus. 
    private IEnumerator BlockMouseoverForDuration(float duration)
    {
        BlockMouseover = true;
        
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            yield return null;
        }
        
        BlockMouseover = false;
    }

    public void Lock()
    {
        lockInteraction = true;
    }

    public void Unlock()
    {
        lockInteraction = false;
    }

    public void SetState(CardState cardState, Vector3? defaultPosition = null)
    {
        state = cardState;
        var relativePosition = defaultPosition ?? DefaultPosition;
        targetPositionRequested = relativePosition + transform.rotation * state.Offset;
    }

    public void TweenToPosition(Vector3 position, float duration, System.Action onComplete = null)
    {
        targetPositionRequested = targetPositionCached = position;
        moveTween?.Cancel(true);
        moveTween = transform.TweenMove(targetPositionRequested, duration, onComplete, Easing.Cubic.Out, 0f);
    }

    public void MoveToRequestedPosition(float duration)
    {

        if (state.Equals(CardState.Examine))
        {
            StartCoroutine(BlockMouseoverForDuration(duration));
            SetPosition(targetPositionRequested);
            return;
        }
        
        if (state.Equals(CardState.ClearFocus))
        {
            SetPosition((transform.position + targetPositionRequested) / 2);
            targetPositionRequested = DefaultPosition;
        }
        
        TweenToPosition(targetPositionRequested, duration);
    }

    private void SetPosition(Vector3 position)
    {
        if (!lockInteraction)
        {
            targetPositionCached = position;
            transform.position = position;
            moveTween?.Cancel(true);
        }
    }

    public bool PlayCard(ICharacter target)
    {
        if (!lockInteraction)
        {
            Lock();
            cardAction.Invoke(target);
            return true;
        }

        return false;
    }

    public Vector3 GetStablePosition()
    {
        return state.Equals(CardState.ClearFocus) ? DefaultPosition : targetPositionRequested;
    }
}