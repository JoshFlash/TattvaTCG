using Cysharp.Threading.Tasks;
using TweenKey;
using TweenKey.Interpolation;
using UnityEngine;

public enum CardState { Default, Examine, Select, DodgeLeft, DodgeRight }

public class Card : MonoBehaviour
{
    private CardConfig cardConfig = null;

    public int Index { get; set; } = -1;
    public bool LockPosition { get; set; } = true;

    private (Vector3 cached, Vector3 requeseted) targetPosition = default;
    private Vector3 defaultPosition = default;
    private Vector3 examinePosition => defaultPosition + new Vector3(-cardConfig.ExamineDodgeDistance, cardConfig.ExamineHeight, 0);
    private Vector3 selectPosition => defaultPosition + new Vector3(-0.5f * defaultPosition.x, cardConfig.SelectHeight, cardConfig.SelectDepth);
    private Vector3 dodgePositionLeft => defaultPosition + new Vector3(-cardConfig.DodgeDistance, 0, 0);
    private Vector3 dodgePositionRight => defaultPosition + new Vector3(cardConfig.DodgeRightDistance, 0, 0);

    private Tween<Vector3> moveTween = null;

    public bool ShouldMove()
    {
        return !LockPosition && targetPosition.cached != targetPosition.requeseted;
    }

    private void Awake()
    {
        LockPosition = true;
        cardConfig = CardConfig.GlobalSettings;
    }

    public void CachePosition()
    {
        defaultPosition = transform.position;
    }

    public void SetState(CardState cardState)
    {
        targetPosition.requeseted = targetPosition.cached;
        switch (cardState)
        {
            case CardState.Default:
                targetPosition.requeseted = defaultPosition;
                break;
            case CardState.Examine:
                targetPosition.requeseted = examinePosition;
                SetPosition(targetPosition.requeseted);
                break;
            case CardState.Select:
                targetPosition.requeseted = selectPosition;
                break;
            case CardState.DodgeLeft:
                targetPosition.requeseted = dodgePositionLeft;
                break;
            case CardState.DodgeRight:
                targetPosition.requeseted = dodgePositionRight;
                break;
        }
    }

    public void TweenToPosition(Vector3 position, float duration, System.Action onComplete = null)
    {
        targetPosition.requeseted = targetPosition.cached = position;
        moveTween?.Cancel(true);

        moveTween = transform.TweenMove(targetPosition.cached, duration, onComplete, Easing.Cubic.Out, 0f);
    }

    public void TweenToRequestedPosition(float duration, System.Action onComplete = null)
    {
        TweenToPosition(targetPosition.requeseted, duration, onComplete);
    }

    private void SetPosition(Vector3 position)
    {
        targetPosition.cached = position;
        transform.position = position;
        moveTween?.Cancel(true);
    }
}