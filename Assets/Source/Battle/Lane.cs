using UnityEngine;

public class Lane : MonoBehaviour, ITarget
{
    public Transform Anchor => this.transform;
    
    [field: SerializeField] public bool IsFriendly { get; private set; }

    bool ITarget.IsFriendly() => this.IsFriendly;

    // todo - implement targeting for all characters in lane
    public Character GetCharacter()
    {
        return null;
    }

    public Lane GetLane()
    {
        return this;
    }
}