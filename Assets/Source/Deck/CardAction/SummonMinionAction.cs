
using UnityEngine;

public class SummonMinionAction : CardAction<(int,int)>
{
    protected override void InvokeOnTarget(in ITarget target, in (int,int) powerHp)
    {
        gameObject.layer = LayerMask.NameToLayer("Units");
        var card = GetComponent<PlayerCard>();
        var lane = target.Lane;
        
        var minion = GetComponent<Minion>();
        var power = minion.BasePower + powerHp.Item1;
        var hp = minion.MaxHealth + powerHp.Item2;

        void OnPlaced()
        {
            card.Lock();
            card.enabled = false;

            minion.enabled = true;
            minion.SetStatsOnSummon(power, hp);
            minion.transform.SetParent(lane.transform);
        }

        var placement = lane.Anchor.position;
        card.TweenToPosition(placement, CardMovementConfig.SummonSpeed, OnPlaced);
    }
}