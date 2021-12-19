
//TODO refactor this
public enum AbilityPriority { Prep, Burst, Spell, Ability, Reaction, Recovery, EndOfTurn }

public class DamageSpell
{
    public string Name;
    public float Damage;
    public AbilityPriority Priority;

    public void Cast(ICharacter target)
    {
        BattleActions.DamageCharacter(target, Damage);
    }
}