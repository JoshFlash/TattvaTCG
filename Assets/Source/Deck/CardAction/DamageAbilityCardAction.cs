using UnityEngine;

public class DamageAbilityCardAction : CardAction<int>
{
    protected override void InvokeOnTarget(in ICharacter target, in int damage)
    {
        BattleActions.DamageCharacter.Invoke(target, damage);
    }
}