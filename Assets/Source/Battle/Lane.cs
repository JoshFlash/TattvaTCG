using UnityEngine;

public class Lane : MonoBehaviour, ITarget
{
    public Transform Anchor => this.transform;
    
    [field: SerializeField] public bool IsFriendly { get; private set; }

    bool ITarget.IsFriendly() => this.IsFriendly;
    Lane ITarget.Lane => this;

    // todo - implement targeting for all characters in lane
}