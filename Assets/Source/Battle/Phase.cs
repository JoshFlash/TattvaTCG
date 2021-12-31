using System;

public record Phase
{
    public int Priority { get; init; }
    public int TotalPhases { get; init; }
    public Phase Next() => new (Priority + 1, TotalPhases);
    
    public Phase(int priority, int totalPhases)
    {
        Priority = priority % totalPhases;
        TotalPhases = totalPhases;
    }
}