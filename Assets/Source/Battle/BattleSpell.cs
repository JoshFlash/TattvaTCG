
public delegate void BattleSpell<in TModifier>(ICardTarget target, TModifier modifier);

public static class BattleSpells
{
    public static BattleSpell<int> DamageCharacter =>
        (target, damage) => (target as Character)!.TakeDamage(damage);
    
    public static BattleSpell<int> HealCharacter => 
        (target, healing) => (target as Character)!.RestoreHealth(healing);
}