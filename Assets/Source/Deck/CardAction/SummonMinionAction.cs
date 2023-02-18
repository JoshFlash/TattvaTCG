
using UnityEngine;

public class SummonMinionAction : CardAction<CombatStats>
{
    // stat modifiers are added to base stats on summon
    protected override void InvokeOnTarget(in ICardTarget target, in CombatStats statModifiers)
    {
        gameObject.layer = LayerMask.NameToLayer("Units");
        var card = GetComponent<PlayerCard>();
        card.ManaCostIcon.SetActive(false);

        var lane = target.Lane;
        
        var minion = GetComponent<Minion>();
        var block = minion.BaseBlock + statModifiers.Block;
        var power = minion.BasePower + statModifiers.Power;
        var hp = minion.MaxHealth + statModifiers.Health;

        void OnPlaced()
        {
            card.Lock();
            card.enabled = false;

            minion.enabled = true;
            minion.SetStatsOnSummon(block, power, hp);
            minion.transform.SetParent(lane.transform);
        }

        var placement = lane.Anchor.position;
        card.TweenToPosition(placement, CardMovementConfig.SummonSpeed, OnPlaced);
    }
}