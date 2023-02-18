using UnityEngine;

public class Lane : MonoBehaviour, ICardTarget
{
    public Transform Anchor => this.transform;
    
    [field: SerializeField] public bool IsFriendly { get; private set; }

    bool ICardTarget.IsFriendly() => this.IsFriendly;
    Lane ICardTarget.Lane => this;

    // todo - implement targeting for all characters in lane
}