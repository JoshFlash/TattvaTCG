using UnityEngine;

public class DamageAbilityCardAction : CardAction<int>
{
    protected override void InvokeOnTarget(in ITarget target, in int damage)
    {
        BattleActions.DamageCharacter.Invoke(target, damage);
    }
}