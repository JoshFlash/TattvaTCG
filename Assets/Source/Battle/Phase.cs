using System;

public class Phase : IEquatable<Phase>
{
    private readonly int priority;
    private readonly int totalPhases;

    public Phase(int priority, int totalPhases)
    {
        this.priority = priority % totalPhases;
        this.totalPhases = totalPhases;
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
        return priority == other.priority && totalPhases == other.totalPhases;
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
        // Cantor pairing function
        return (priority + totalPhases) * (priority + totalPhases + 1) / 2 + priority;
    }
}