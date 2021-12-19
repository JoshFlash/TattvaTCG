using System;

public class Phase : IEquatable<Phase>
{
    private const int TOTAL_PHASES = 5;
    
    private readonly int priority = 0;
    
    public Phase(int priority)
    {
        this.priority = priority % TOTAL_PHASES;
    }

    public static readonly Phase Prep = new (0);
    public static readonly Phase Burst = new (1);
    public static readonly Phase Spell = new (2);
    public static readonly Phase Ability = new (3);
    public static readonly Phase Recovery = new (4);

    public static Phase operator ++(Phase phase)
    {
        return new Phase(phase.priority + 1);
    }

    public bool Equals(Phase other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return priority == other.priority;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Phase)obj);
    }

    public override int GetHashCode()
    {
        return priority;
    }
}