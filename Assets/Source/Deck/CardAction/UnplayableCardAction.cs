using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnplayableCardAction : CardAction<int>
{
    protected override void InvokeOnTarget(in ICharacter target, in int damage)
    {
        // do nothing
    }
}
