using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnplayableCardAction : CardAction<int>
{
    protected override void InvokeOnTarget(in ITarget target, in int damage)
    {
        // do nothing
    }
}
