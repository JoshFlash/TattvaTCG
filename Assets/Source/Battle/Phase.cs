using System;

public class Phase : IEquatable<Phase>, IComparable<Phase>
{
    private readonly int priority = 0;
    private readonly int totalPhases = 0;

    public Phase(int priority, int totalPhases)
    {
        this.priority = priority % totalPhases;
    }

    public static Phase operator ++(Phase phase)
    {
        return new Phase(phase.priority + 1, phase.totalPhases);
    }

    public Phase Next()
    {
        return new Phase(priority + 1, totalPhases);
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

    public int CompareTo(Phase other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return priority.CompareTo(other.priority);
    }
}