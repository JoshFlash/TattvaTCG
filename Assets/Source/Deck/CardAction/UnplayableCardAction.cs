using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnplayableCardAction : CardAction<int>
{
    protected override void InvokeOnTarget(in ICardTarget target, in int blockPowerHp)
    {
        // do nothing
    }
}
