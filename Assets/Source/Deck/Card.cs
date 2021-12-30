using Cysharp.Threading.Tasks;
using TweenKey;
using TweenKey.Interpolation;
using UnityEngine;

public enum CardState { Default, Examine, Select, DodgeLeft, DodgeRight }

public class Card : MonoBehaviour
{
    private const float DEFAULT_MOVE_SPEED = 0.21f;
    private CardConfig cardConfig = CardConfig.GlobalSettings;

    public int Index { get; set; } = -1;
    public bool LockPosition { get; set; } = true;

    private Vector3 targetPosition = default;
    private Vector3 defaultPosition = default;
    private Vector3 examinePosition => defaultPosition + new Vector3(-cardConfig.ExamineDodgeDistance, cardConfig.ExamineHeight, 0);
    private Vector3 selectPosition => defaultPosition + new Vector3(-0.5f * defaultPosition.x, cardConfig.SelectHeight, cardConfig.SelectDepth);
    private Vector3 dodgePositionLeft => defaultPosition + new Vector3(-cardConfig.DodgeDistance, 0, 0);
    private Vector3 dodgePositionRight => defaultPosition + new Vector3(cardConfig.DodgeRightDistance, 0, 0);

    private bool ShouldMove(Vector3 position)
    {
        return !LockPosition && position != targetPosition;
    }

    private void Awake()
    {
        LockPosition = true;
    }

    public void CachePosition()
    {
        defaultPosition = transform.position;
    }

    public void SetState(CardState cardState)
    {
        var position = targetPosition;
        switch (cardState)
        {
            case CardState.Default:
                position = defaultPosition;
                break;
            case CardState.Examine:
                position = examinePosition;
                break;
            case CardState.Select:
                position = selectPosition;
                break;
            case CardState.DodgeLeft:
                position = dodgePositionLeft;
                break;
            case CardState.DodgeRight:
                position = dodgePositionRight;
                break;
        }

        if (ShouldMove(position))
        {
            MoveToPosition(position);
        }
    }

    public void MoveToPosition(Vector3 position, float speed = DEFAULT_MOVE_SPEED, System.Action onComplete = null)
    {
        targetPosition = position;
        transform.TweenMove(targetPosition, 1 / speed, onComplete, Easing.Cubic.Out, 0f);
    }
}