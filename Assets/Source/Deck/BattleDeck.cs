using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleDeck
{
    public BattleDeck(/* [Deck Asset] */)
    {
        
    }
    
    public PlayerHand PlayerHand { get; } = new();

    private List<PlayerCard> deck = new();
    private Queue<PlayerCard> drawPile = new();
    private Stack<PlayerCard> discardPile = new();
    private List<PlayerCard> banishedCards = new();

    public async UniTask<bool> AddCardToHand(HandInputHandler handInputHandler, Transform handAnchor)
    {
        if (PlayerHand.IsFull) return false;
        
        var cardPrefab = GameServices.Get<DebugService>().BattleDebugData.PlayerDefaultCardInDeck.GetComponent<PlayerCard>();
        var cardParent = handAnchor.transform;
        PlayerCard playerCard = GameObject.Instantiate(cardPrefab, cardParent.position, cardParent.rotation, cardParent);
        
        PlayerHand.Add(playerCard);
        await handInputHandler.AddAndAdjust(playerCard, handAnchor);
        return true;
    }
}