
public delegate void BattleAction<in TTarget, in TModifier>(TTarget target, TModifier modifier);

public static class BattleActions
{
    public static BattleAction<ICharacter, int> DamageCharacter => (character, damage) => character.TakeDamage(damage);
    public static BattleAction<ICharacter, int> HealCharacter => (character, healing) => character.RestoreHealth(healing);
}