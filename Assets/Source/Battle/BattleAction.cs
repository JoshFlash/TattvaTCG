
public delegate void BattleAction<in TTarget, in TModifier>(TTarget target, TModifier modifier);

public static class BattleActions
{
    public static BattleAction<ITarget, int> DamageCharacter => 
        (target, damage) => target.GetCharacter()?.TakeDamage(damage);
    
    public static BattleAction<ITarget, int> HealCharacter => 
        (target, healing) => target.GetCharacter()?.RestoreHealth(healing);
}