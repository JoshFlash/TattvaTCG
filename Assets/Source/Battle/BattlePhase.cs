
public static class BattlePhase
{
    private const int TOTAL_PHASES = 5;
        
    public static readonly Phase Prep = new (0, TOTAL_PHASES);
    public static readonly Phase Burst = Prep.Next();
    public static readonly Phase Spell = Burst.Next();
    public static readonly Phase Ability = Spell.Next();
    public static readonly Phase Recovery = Ability.Next();
    
}