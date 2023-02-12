
using UnityEngine;

public class SummonMinionAction : CardAction<(int,int)>
{
    protected override void InvokeOnTarget(in ITarget target, in (int,int) atk_hp)
    {
        gameObject.layer = LayerMask.NameToLayer("Units");
        var card = GetComponent<PlayerCard>();
        var minion = GetComponent<Minion>();

        void OnPlaced()
        {
            minion.enabled = true;
            card.enabled = false;
        }
        
        var placement = GetCardPlacementLocation(target?.GetLane());
        card.TweenToPosition(placement, CardMovementConfig.SummonSpeed, OnPlaced);
    }

    private Vector3 GetCardPlacementLocation(Lane lane)
    {
        return lane.Anchor.position;
    }
}