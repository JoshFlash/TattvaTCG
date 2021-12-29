using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class BattlePhase
{
    private const int TOTAL_PHASES = 5;

    public static readonly Phase Prep       = new (0, TOTAL_PHASES);
    public static readonly Phase Burst      = new (1, TOTAL_PHASES);
    public static readonly Phase Spell      = new (2, TOTAL_PHASES);
    public static readonly Phase Ability    = new (3, TOTAL_PHASES);
    public static readonly Phase Recovery   = new (4, TOTAL_PHASES);

    public static IReadOnlyDictionary<Phase, string> BattlePhaseAliases = new Dictionary<Phase, string>
    {
        {Prep, "Prep"},
        {Burst, "Burst"},
        {Spell, "Spell"},
        {Ability, "Ability"},
        {Recovery, "Recovery"},
    };

}