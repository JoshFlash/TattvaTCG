
public delegate void BattleAction<in TModifier>(ITarget target, TModifier modifier);

public static class BattleActions
{
    public static BattleAction<int> DamageCharacter =>
        (target, damage) => (target as Character)!.TakeDamage(damage);
    
    public static BattleAction<int> HealCharacter => 
        (target, healing) => (target as Character)!.RestoreHealth(healing);
}