using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleDeckController : MonoBehaviour
{
    [SerializeField] private HandController handController = default;
    
    public async UniTask<Card> AddCardToHand()
    {
        return await handController.AddCard();
    }

    public void UnlockHand()
    {
        handController.UnlockAllCards();
    }

    public void ClearHand()
    {
        handController.ClearAllCards();
    }
}