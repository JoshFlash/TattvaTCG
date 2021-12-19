
public delegate void BattleAction<in TTarget, in TModifier>(TTarget target, TModifier modifier);

public static class BattleActions
{
    public static BattleAction<ICharacter, float> DamageCharacter => (character, damage) => character.TakeDamage(damage);
}