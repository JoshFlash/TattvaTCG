using System;
using UnityEngine;

public struct CardState : IEquatable<CardState>
{
    private static Vector3 kDefaultOffset       => Vector3.zero;
    private static Vector3 kExamineOffset       => new(0, CardMovementConfig.ExamineHeight, CardMovementConfig.ExamineDepth);
    private static Vector3 kClearOffset         => new(0, CardMovementConfig.ExamineHeight / 2, 0);
    private static Vector3 kSelectOffset        => new(0, CardMovementConfig.SelectHeight, CardMovementConfig.SelectDepth);
    private static Vector3 kTargetOffset        => new(0, CardMovementConfig.ExamineHeight, CardMovementConfig.SelectDepth);
    private static Vector3 kDodgeOffsetRight    => new(-CardMovementConfig.DodgeDistance, 0, 0);
    private static Vector3 kDodgeOffsetLeft     => new(CardMovementConfig.DodgeDistance, 0, 0);
    
    public Vector3 Offset { get; init; }
    public int Id { get; init; }
    

    private static readonly Lazy<CardState> kDefault = new(() => new() { Offset = kDefaultOffset, Id = 0 } );
    public static CardState Default => kDefault.Value;
    

    private static readonly Lazy<CardState> kDodgeRight = new(() => new() { Offset = kDodgeOffsetRight, Id = 1 } );
    public static CardState DodgeRight => kDodgeRight.Value;
    

    private static readonly Lazy<CardState> kDodgeLeft = new(() => new() { Offset = kDodgeOffsetLeft, Id = 2 } );
    public static CardState DodgeLeft => kDodgeLeft.Value;
    
    
    private static readonly Lazy<CardState> kExamine = new(() => new() { Offset = kExamineOffset, Id = 3 } );
    public static CardState Examine => kExamine.Value;
    

    private static readonly Lazy<CardState> kClear = new(() => new() { Offset = kClearOffset, Id = 4 } );
    public static CardState ClearFocus => kClear.Value;
    

    private static readonly Lazy<CardState> kSelect = new(() => new() { Offset = kSelectOffset, Id = 5 } );
    public static CardState Select => kSelect.Value;
    
    
    private static readonly Lazy<CardState> kTarget = new(() => new() { Offset = kTargetOffset, Id = 6 } );
    public static CardState Target => kTarget.Value;


    public bool Equals(CardState other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        return obj is CardState other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Offset.GetHashCode() * 397) ^ Id;
        }
    }
}