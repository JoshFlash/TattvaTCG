
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

        var modifiers = statModifiers;

        void OnPlaced()
        {
            card.Lock();
            card.enabled = false;

            minion.enabled = true;
            minion.SetStatsOnSummon(modifiers);
            minion.transform.SetParent(lane.transform);
            
            lane.Minions.Add(minion);
        }

        var offset = (lane.Minions.Count * CardMovementConfig.MaxPadding * Vector3.left) +
                     (CardMovementConfig.DepthInterval * lane.Minions.Count * Vector3.back);
        var placement = lane.Anchor.position + offset;
        card.TweenToPosition(placement, CardMovementConfig.SummonSpeed, OnPlaced);
    }
    
    public override bool CanTarget(ICardTarget target)
    {
        return base.CanTarget(target)
               && (target.Lane is not null)
               && (target.Lane.Minions.Count < Lane.kMaxMinionsInLane);
    }
}