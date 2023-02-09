using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleDeckController : MonoBehaviour
{
    public async UniTask<PlayerCard> AddCardToHand(HandController handController)
    {
        return await handController.AddCard();
    }
}