using System.Collections.Generic;
using System.Threading.Tasks;

public class Phase
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
    public static readonly Phase Recovery = new (3);

    public static Phase operator ++(Phase phase)
    {
        return new Phase(phase.priority + 1);
    }
}