using UnityEngine;

public class DamageAbilityCardAction : CardAction<int>
{
    protected override void InvokeOnTarget(in ICardTarget target, in int blockPowerHp)
    {
        BattleSpells.DamageCharacter.Invoke(target, blockPowerHp);
    }
}