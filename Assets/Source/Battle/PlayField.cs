using UnityEngine;

public class PlayField : MonoBehaviour
{
    [field: SerializeField] public Transform PlayerAnchor { get; private set; } = default;
    [field: SerializeField] public Transform OpponentAnchor { get; private set; } = default;
    
    [field: SerializeField] public Lane PlayerTopLane { get; private set; } = default;
    [field: SerializeField] public Lane PlayerBottomLane { get; private set; } = default;

    [field: SerializeField] public Lane OpponentTopLane { get; private set; } = default;
    [field: SerializeField] public Lane OpponentBottomLane { get; private set; } = default;
}

