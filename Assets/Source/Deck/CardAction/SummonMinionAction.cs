
using UnityEngine;

public class SummonMinionAction : CardAction<int>
{
    protected override void InvokeOnTarget(in ICharacter target, in int lane)
    {
        gameObject.layer = LayerMask.NameToLayer("Units");
        var card = GetComponent<PlayerCard>();
        var minion = GetComponent<Minion>();

        void OnPlaced()
        {
            minion.enabled = true;
            card.enabled = false;
        }
        
        var placement = GetCardPlacementLocation(lane);
        card.TweenToPosition(placement, CardMovementConfig.SummonSpeed, OnPlaced);
    }

    private Vector3 GetCardPlacementLocation(int lane)
    {
        var playField = GameServices.Get<BattleService>().PlayField;
        var laneAnchor = lane == 0 ? playField.PlayerTopLane.Anchor : playField.PlayerBottomLane.Anchor;
        return laneAnchor.position;
    }
}